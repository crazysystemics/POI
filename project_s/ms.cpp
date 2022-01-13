class MS
{
	//Only Discrete is Modelled
	//GIT Test
	MS_Core[num_In_Signal, num_WARNING]
	
	Warning getWarning(bit[] inVector);
}

static class RVGS
{
	MS Known_Good_Unit;//Black Box
    MS Good_Like_Unit
	
	//stage 01
	void Generate_Good_Like_Unit ()
	{
		//----srinivas --develop rgvs.c
		//----void generate_lru_model(lru_id, lru_inputs, lru_outputs) { }
			//----lru_inputs
				//interface_type {DISCRETE, ANALOG, SERIAL, ARINC, 1553B}
				//----engineering name
				//----engineering value
				//----range(min,max)
				//----encoding_type(BCNR)
				//----number_of_bits
				//----incoming data frequency
			//----lru_outputs
				//interface_type {DISCRETE, ANALOG, SERIAL, ARINC, 1553B}
				//----engineering name
				//----engineering value
				//----range(min,max)
				//----encoding_type(BCNR)
				//----number_of_bits
				//----incoming data frequency
		     //----map[lru_inputs, lru_outputs]
			
		while (results_not_converged)
		{
			bit[] stimuls = generateNextStimulus();
			Warning w = Known_Good_Unit.getWarning(stimulus);
			Good_Like_Unit[w, stimulus] = RED/YELLOW/GREEN;
			resolve_conflicts();
		}
	}
	
	//stage 02
	void ReconcileGaps (MS Known_Good_Unit, MS Good_Like_Unit)
	{
		//Manual work to correct functional block (excel file)
	}
}

//stage 03
static class ATS
{
	MS UUT;
	AcceptanceTestPlan ATP;
	
	void doAcceptanceTest()
	{
		InjectSignalsIntoUUT();
		ReadBackSignals();
		Verify();
	}

}

//stage 04
class IVR
{
	MS Known_Good_Unit, UUT;
	
	int Verify()
	{
		foreach(InputVector iv in Known_Good_Unit.inputs)
		{
			Warning kgu_warning = Known_Good_Unit.getWarning(iv);
			Warning uut_warning = UUT.getWarning(iv);
			if (kgu_warning != uut_warning)
			{
				return 0;
			}
		}
		return 1;
	}
}

void main()
{
	MS ms = new MS(csv_file);
	MS new_unit = new MS(empty_csv_file);
	
	//stage 01
	RVGS.Known_Good_Unit = ms;
	RVGS.Good_Like_Unit  = new_unit;
	RVGS.Good_Like_Unit;
	
	//stage 02
	RVGS.ReconcileGaps(Known_Good_Unit, Good_Like_Unit);
	
	//stage 03
	ATS ats = new ATS();
	ats.UUT = RVGS.Good_Like_Unit;
	ats.doAcceptanceTest();
	
	//stage 04
	IVR ivr = new IVR();
	ivr.Verify();
	
}
	
	
	
	