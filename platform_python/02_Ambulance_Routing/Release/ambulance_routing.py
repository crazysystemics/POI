"""
Ambulance Routing: Multi-Agent Traffic Light Coordination
=========================================================

Compares three coordination strategies across N_RUNS independent runs:
  1. Naive          -- no coordination; ambulance waits at red lights
  2. Centralized    -- dispatcher pre-clears full route (ETA-based holds)
  3. Wave           -- rolling green: only the immediate next node locked

Metric: average vehicle wait ticks per background vehicle.
EMERGENCY_GREEN blocks cross-traffic vehicles (realistic disruption model).

Town: 20-node grid (4 cols × 5 rows) giving an ~8-node ambulance path,
long enough for Wave's narrow 1-node lock to clearly outperform Centralized.
"""

import random
import networkx as nx
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.patches as mpatches
from enum import Enum


# ─────────────────────────────────────────────────────────────────
# CONFIG
# ─────────────────────────────────────────────────────────────────
LIGHT_CYCLE     = 14      # ticks per full RED/GREEN cycle (7 each)
AMB_SPEED       = 0.28    # fraction of edge per tick  (~4 ticks/edge)
VEH_SPEED_LO    = 0.06
VEH_SPEED_HI    = 0.18
N_VEHICLES      = 40      # more vehicles → more reliable average
SIM_TICKS       = 300     # ticks per run
N_RUNS          = 10      # independent runs to average over
EMERG_HOLD_BASE = 14      # centralized base hold (refined by ETA formula)
WAVE_HOLD       = int(1.0 / AMB_SPEED) + 1   # = 4 ticks; just enough per edge
WAVE_LOOKAHEAD  = 1       # only immediate next node — minimum needed
VEHICLE_SEED    = 99      # same vehicle layout across all 3 strategies


# ─────────────────────────────────────────────────────────────────
# ENUMS
# ─────────────────────────────────────────────────────────────────
class LightState(Enum):
    RED             = ("#c0392b", "R")
    GREEN           = ("#27ae60", "G")
    EMERGENCY_GREEN = ("#00ff99", "E")

class Strategy(Enum):
    NAIVE       = "Naive"
    CENTRALIZED = "Centralized"
    WAVE        = "Wave"


# ─────────────────────────────────────────────────────────────────
# TOWN GRAPH  — 20-node grid (4 cols × 5 rows)
# ─────────────────────────────────────────────────────────────────
#
#   N00 - N01 - N02 - N03
#    |     |     |     |
#   N10 - N11 - N12 - N13
#    |     |     |     |
#   N20 - N21 - N22 - N23
#    |     |     |     |
#   N30 - N31 - N32 - N33
#    |     |     |     |
#   N40 - N41 - N42 - HOSP
#
#  Ambulance: N00 (top-left)  →  HOSP (bottom-right)
#  Dijkstra path crosses ~8 intersections — long enough to show
#  the difference between Wave (1 node locked) and Centralized (all locked)

ROWS, COLS = 5, 4

def node_name(r, c):
    if r == ROWS - 1 and c == COLS - 1:
        return "HOSP"
    return f"N{r}{c}"

def build_town() -> nx.Graph:
    G = nx.Graph()

    # nodes with (x, y) positions
    for r in range(ROWS):
        for c in range(COLS):
            name = node_name(r, c)
            G.add_node(name, pos=(c * 2.0, (ROWS - 1 - r) * 2.0))

    # horizontal edges
    for r in range(ROWS):
        for c in range(COLS - 1):
            w = random.randint(2, 4)
            G.add_edge(node_name(r, c), node_name(r, c + 1), weight=w)

    # vertical edges
    for r in range(ROWS - 1):
        for c in range(COLS):
            w = random.randint(2, 4)
            G.add_edge(node_name(r, c), node_name(r + 1, c), weight=w)

    return G


# Build graph once with a fixed seed so topology is reproducible
random.seed(7)
TOWN:     nx.Graph = build_town()
NODE_POS: dict     = nx.get_node_attributes(TOWN, "pos")
NODES:    list     = list(TOWN.nodes())
START,    DEST     = "N00", "HOSP"


# ─────────────────────────────────────────────────────────────────
# TRAFFIC LIGHT AGENT
# ─────────────────────────────────────────────────────────────────
class TrafficLightAgent:
    def __init__(self, node_id: str, phase_offset: int = 0):
        self.node_id    = node_id
        self._tick      = phase_offset
        half            = LIGHT_CYCLE // 2
        self.state      = LightState.GREEN if phase_offset < half else LightState.RED
        self._emerg_rem = 0

    def tick(self):
        if self._emerg_rem > 0:
            self._emerg_rem -= 1
            if self._emerg_rem == 0:
                self.state = LightState.RED
            return
        self._tick += 1
        self.state = (LightState.GREEN
                      if (self._tick % LIGHT_CYCLE) < LIGHT_CYCLE // 2
                      else LightState.RED)

    def set_emergency(self, duration: int):
        """Centralized: never shorten (one-shot pre-clear)."""
        self._emerg_rem = max(self._emerg_rem, duration)
        self.state      = LightState.EMERGENCY_GREEN

    def set_wave_emergency(self, duration: int):
        """Wave: always overwrite — no hold accumulation."""
        self._emerg_rem = duration
        self.state      = LightState.EMERGENCY_GREEN

    def is_passable(self) -> bool:
        return self.state in (LightState.GREEN, LightState.EMERGENCY_GREEN)

    def is_passable_for_vehicle(self) -> bool:
        """Cross-traffic blocked by EMERGENCY_GREEN."""
        return self.state == LightState.GREEN


# ─────────────────────────────────────────────────────────────────
# BACKGROUND VEHICLE
# ─────────────────────────────────────────────────────────────────
class BackgroundVehicle:
    def __init__(self, vid: int, start: str):
        self.cur        = start
        self.nxt        = self._pick(start)
        self.progress   = random.random()
        self.speed      = random.uniform(VEH_SPEED_LO, VEH_SPEED_HI)
        self.wait_ticks = 0

    @staticmethod
    def _pick(node: str) -> str:
        nbrs = list(TOWN.neighbors(node))
        return random.choice(nbrs) if nbrs else node

    def move(self, lights: dict):
        if self.progress > 0.80 and not lights[self.nxt].is_passable_for_vehicle():
            self.wait_ticks += 1
            return
        self.progress += self.speed
        if self.progress >= 1.0:
            self.progress = 0.0
            self.cur      = self.nxt
            self.nxt      = self._pick(self.cur)

    def xy(self) -> np.ndarray:
        p0 = np.array(NODE_POS[self.cur])
        p1 = np.array(NODE_POS[self.nxt])
        return p0 + self.progress * (p1 - p0)


# ─────────────────────────────────────────────────────────────────
# AMBULANCE AGENT
# ─────────────────────────────────────────────────────────────────
class AmbulanceAgent:
    def __init__(self, start: str, dest: str):
        self.path        = nx.shortest_path(TOWN, start, dest, weight="weight")
        self.path_idx    = 0
        self.cur         = start
        self.progress    = 0.0
        self.arrived     = False
        self.wait_ticks  = 0
        self.total_ticks = 0

    def peek_next(self):
        if self.path_idx + 1 < len(self.path):
            return self.path[self.path_idx + 1]
        return None

    def move(self, lights: dict):
        if self.arrived:
            return
        self.total_ticks += 1
        nxt = self.peek_next()
        if nxt is None:
            self.arrived = True
            return
        if self.progress == 0.0 and not lights[nxt].is_passable():
            self.wait_ticks += 1
            return
        self.progress += AMB_SPEED
        if self.progress >= 1.0:
            self.progress = 0.0
            self.path_idx += 1
            self.cur      = self.path[self.path_idx]
            if self.cur   == self.path[-1]:
                self.arrived = True

    def xy(self) -> np.ndarray:
        nxt = self.peek_next()
        if nxt is None or self.arrived:
            return np.array(NODE_POS[self.cur], dtype=float)
        return (np.array(NODE_POS[self.cur]) +
                self.progress * (np.array(NODE_POS[nxt]) -
                                 np.array(NODE_POS[self.cur])))


# ─────────────────────────────────────────────────────────────────
# COORDINATION STRATEGIES
# ─────────────────────────────────────────────────────────────────
def centralized_init(ambulance: AmbulanceAgent, lights: dict):
    """
    ETA-based pre-clear: hold[i] = (i+1)*ticks_per_edge + 2
    Each node locked only as long as needed for the ambulance to
    arrive and pass through — no blanket over-locking.
    """
    ticks_per_edge = int(1.0 / AMB_SPEED) + 1
    for i, node in enumerate(ambulance.path):
        hold = (i + 1) * ticks_per_edge + 2
        lights[node].set_emergency(hold)


def wave_tick(ambulance: AmbulanceAgent, lights: dict):
    """
    Lock only the immediately next node (WAVE_LOOKAHEAD=1).
    WAVE_HOLD=4 ticks > traversal time — guarantees zero ambulance wait.
    Minimal footprint: only 1 intersection locked at any moment.
    """
    idx = ambulance.path_idx
    for k in range(1, WAVE_LOOKAHEAD + 1):
        fi = idx + k
        if fi < len(ambulance.path):
            lights[ambulance.path[fi]].set_wave_emergency(WAVE_HOLD)


# ─────────────────────────────────────────────────────────────────
# SIMULATION
# ─────────────────────────────────────────────────────────────────
class Simulation:
    def __init__(self, strategy: Strategy, run_seed: int):
        self.strategy = strategy
        self.tick     = 0

        # traffic lights — staggered phases
        self.lights = {
            n: TrafficLightAgent(n, phase_offset=(i * 3) % LIGHT_CYCLE)
            for i, n in enumerate(NODES)
        }
        self.lights[DEST].set_emergency(99999)

        self.ambulance = AmbulanceAgent(START, DEST)

        # identical vehicle layout across all strategies in the same run
        random.seed(VEHICLE_SEED + run_seed)
        self.vehicles = [
            BackgroundVehicle(i, random.choice(NODES))
            for i in range(N_VEHICLES)
        ]

        if strategy == Strategy.CENTRALIZED:
            centralized_init(self.ambulance, self.lights)

    def avg_vehicle_wait(self) -> float:
        return sum(v.wait_ticks for v in self.vehicles) / len(self.vehicles)

    def step(self):
        self.tick += 1
        for lt in self.lights.values():
            lt.tick()
        if self.strategy == Strategy.WAVE and not self.ambulance.arrived:
            wave_tick(self.ambulance, self.lights)
        self.ambulance.move(self.lights)
        for v in self.vehicles:
            v.move(self.lights)


# ─────────────────────────────────────────────────────────────────
# MULTI-RUN COMPARISON
# ─────────────────────────────────────────────────────────────────
def run_comparison():
    """
    Run all 3 strategies N_RUNS times with different vehicle seeds.
    Returns per-strategy lists of avg_vehicle_wait and amb_wait_ticks.
    """
    results = {s: {"veh_wait": [], "amb_wait": [], "amb_total": []}
               for s in Strategy}

    path_len = len(nx.shortest_path(TOWN, START, DEST, weight="weight"))
    print(f"Town   : {len(NODES)} nodes, {TOWN.number_of_edges()} edges")
    print(f"Route  : {START} → {DEST}  ({path_len} nodes, "
          f"{path_len-1} edges)")
    print(f"Runs   : {N_RUNS}  ×  {SIM_TICKS} ticks each")
    print()

    for run in range(N_RUNS):
        for strat in Strategy:
            sim = Simulation(strat, run_seed=run)
            for _ in range(SIM_TICKS):
                sim.step()
            r = results[strat]
            r["veh_wait"].append(sim.avg_vehicle_wait())
            r["amb_wait"].append(sim.ambulance.wait_ticks)
            r["amb_total"].append(sim.ambulance.total_ticks)

    return results


# ─────────────────────────────────────────────────────────────────
# VISUALISATION — static bar chart (no animation)
# ─────────────────────────────────────────────────────────────────
FIG_BG   = "#0d1117"
PANEL_BG = "#161b22"
BAR_COLS = {"NAIVE": "#e74c3c", "CENTRALIZED": "#e67e22", "WAVE": "#2ecc71"}


def plot_results(results: dict):
    strats     = list(Strategy)
    names      = [s.value for s in strats]
    veh_means  = [np.mean(results[s]["veh_wait"])  for s in strats]
    veh_stds   = [np.std(results[s]["veh_wait"])   for s in strats]
    amb_means  = [np.mean(results[s]["amb_wait"])  for s in strats]
    total_means= [np.mean(results[s]["amb_total"]) for s in strats]
    colors     = [BAR_COLS[s.name] for s in strats]

    fig, axes = plt.subplots(1, 3, figsize=(18, 6))
    fig.patch.set_facecolor(FIG_BG)
    fig.suptitle(
        f"Ambulance Routing — Strategy Comparison  "
        f"({N_RUNS} runs × {SIM_TICKS} ticks, {len(NODES)}-node town)",
        color="white", fontsize=14, fontweight="bold", y=1.01
    )

    def style_ax(ax, title, ylabel):
        ax.set_facecolor(PANEL_BG)
        ax.set_title(title, color="white", fontsize=11, fontweight="bold", pad=8)
        ax.set_ylabel(ylabel, color="white", fontsize=10)
        ax.set_xticks(range(len(names)))
        ax.set_xticklabels(names, color="white", fontsize=12, fontweight="bold")
        ax.tick_params(axis="y", colors="white")
        ax.spines["bottom"].set_color("#444466")
        ax.spines["left"].set_color("#444466")
        ax.spines["top"].set_visible(False)
        ax.spines["right"].set_visible(False)
        ax.grid(axis="y", color="#30363d", linewidth=0.8, zorder=0)

    # ── Panel 1: avg vehicle wait (the disruption metric) ─────────
    ax = axes[0]
    style_ax(ax, "Avg Vehicle Wait Ticks\n(lower = less disruption to traffic)",
             "ticks / vehicle")
    bars = ax.bar(range(len(names)), veh_means, color=colors,
                  width=0.5, zorder=3, edgecolor="#0d1117",
                  yerr=veh_stds, capsize=6,
                  error_kw={"ecolor": "white", "linewidth": 1.5})
    ax.set_ylim(0, max(veh_means) * 1.4)
    for i, (v, s) in enumerate(zip(veh_means, veh_stds)):
        ax.text(i, v + s + max(veh_means)*0.03, f"{v:.1f}±{s:.1f}",
                ha="center", color="white", fontsize=10, fontweight="bold")

    # ── Panel 2: ambulance wait ticks ─────────────────────────────
    ax = axes[1]
    style_ax(ax, "Ambulance Wait Ticks\n(lower = faster response)",
             "ticks waited at red")
    bars2 = ax.bar(range(len(names)), amb_means, color=colors,
                   width=0.5, zorder=3, edgecolor="#0d1117")
    ax.set_ylim(0, max(amb_means) * 1.5 + 1)
    for i, v in enumerate(amb_means):
        ax.text(i, v + max(amb_means)*0.05 + 0.2, f"{v:.1f}",
                ha="center", color="white", fontsize=10, fontweight="bold")

    # ── Panel 3: total ambulance journey time ─────────────────────
    ax = axes[2]
    style_ax(ax, "Total Ambulance Journey\n(ticks from dispatch to hospital)",
             "total ticks")
    bars3 = ax.bar(range(len(names)), total_means, color=colors,
                   width=0.5, zorder=3, edgecolor="#0d1117")
    ax.set_ylim(0, max(total_means) * 1.3)
    for i, v in enumerate(total_means):
        ax.text(i, v + max(total_means)*0.03, f"{v:.1f}",
                ha="center", color="white", fontsize=10, fontweight="bold")

    # ── annotation: wave advantage ────────────────────────────────
    cent_wait = veh_means[1]
    wave_wait = veh_means[2]
    saving_pct = (cent_wait - wave_wait) / cent_wait * 100 if cent_wait > 0 else 0
    fig.text(
        0.5, -0.04,
        f"Wave vs Centralized vehicle disruption:  "
        f"{'↓' if saving_pct > 0 else '↑'}{abs(saving_pct):.1f}% "
        f"{'less' if saving_pct > 0 else 'more'} collateral wait   |   "
        f"Ambulance path: {' → '.join(nx.shortest_path(TOWN, START, DEST, weight='weight'))}",
        ha="center", color="#aaaacc", fontsize=9, style="italic"
    )

    plt.tight_layout()
    plt.savefig("routing_comparison.png",
                dpi=150, bbox_inches="tight",
                facecolor=FIG_BG)
    print("Chart saved → routing_comparison.png")
    plt.show()


# ─────────────────────────────────────────────────────────────────
if __name__ == "__main__":
    random.seed(42)
    results = run_comparison()

    # Print summary table
    print(f"{'Strategy':<14} {'Avg Veh Wait':>14} {'Amb Wait':>10} {'Journey':>10}")
    print("─" * 52)
    for s in Strategy:
        vw = np.mean(results[s]["veh_wait"])
        aw = np.mean(results[s]["amb_wait"])
        jt = np.mean(results[s]["amb_total"])
        print(f"{s.value:<14} {vw:>13.2f}  {aw:>9.1f}  {jt:>9.1f}")

    plot_results(results)
