def main():
    # Content for the file
    content = """ವಕ್ರತುಂಡ ಮಹಾಕಾಯ ಸೂರ್ಯಕೋಟಿ ಸಮಪ್ರಭ |
ನಿರ್ವಿಘ್ನಂ ಕುರು ಮೇ ದೇವ ಸರ್ವಕಾರ್ಯೇಷು ಸರ್ವದಾ ||

--- ನಿಘಂಟು (ಪ್ರತಿಪದಾರ್ಥ) ---

೧. ವಕ್ರತುಂಡ: ವಕ್ರವಾದ (ಸೊಟ್ಟಗಿರುವ) ತುಂಡ (ಸೊಂಡಿಲು) ಉಳ್ಳವನೇ
೨. ಮಹಾಕಾಯ: ಬೃಹತ್ತಾದ ಕಾಯ (ಶರೀರ) ಉಳ್ಳವನೇ
೩. ಸೂರ್ಯಕೋಟಿ: ಕೋಟಿ ಸೂರ್ಯರಿಗೆ
೪. ಸಮಪ್ರಭ: ಸಮಾನವಾದ ಪ್ರಭೆ (ಕಾಂತಿ/ಬೆಳಕು) ಉಳ್ಳವನೇ
೫. ದೇವ: ಎಲೈ ದೇವನೇ (ಗಣಪತಿಯೇ)
೬. ಮೇ: ನನಗೆ / ನನ್ನ (ಚತುರ್ಥೀ ಅಥವಾ ಷಷ್ಠೀ ವಿಭಕ್ತಿ)
೭. ನಿರ್ವಿಘ್ನಂ: ವಿಘ್ನಗಳಿಲ್ಲದಂತೆ (ಅಡೆತಡೆಗಳಿಲ್ಲದೆ)
೮. ಕುರು: ಮಾಡು
೯. ಸರ್ವಕಾರ್ಯೇಷು: ಎಲ್ಲಾ ಕಾರ್ಯಗಳಲ್ಲಿ (ಕೆಲಸಗಳಲ್ಲಿ)
೧೦. ಸರ್ವದಾ: ಯಾವಾಗಲೂ

--- ಸಾರಾಂಶ ---

ಕೋಟಿ ಸೂರ್ಯರ ಪ್ರಕಾಶಕ್ಕೆ ಸಮನಾದ ಕಾಂತಿಯುಳ್ಳ, ಬೃಹತ್ ಶರೀರದ ವಕ್ರತುಂಡ ಗಣಪತಿಯೇ, ನನ್ನ ಎಲ್ಲಾ ಕಾರ್ಯಗಳೂ ಯಾವಾಗಲೂ ಅಡೆತಡೆಗಳಿಲ್ಲದೆ ನಡೆಯುವಂತೆ ಅನುಗ್ರಹಿಸು.
"""

    # Writing to file
    with open('nighantu.txt', 'w', encoding='utf-8') as f:
        f.write(content)

    # Build dictionary from ನಿಘಂಟು section: strip leading number+dot, split on first ':'
    nighantu = {}
    for line in content.splitlines():
        if ':' not in line:
            continue
        # Drop leading number and dot (e.g. "೧. ")
        stripped = line.split('. ', 1)[-1] if '. ' in line else line
        term, meaning = stripped.split(':', 1)
        nighantu[term.strip()] = meaning.strip()

    print(nighantu)


if __name__ == "__main__":
    main()
