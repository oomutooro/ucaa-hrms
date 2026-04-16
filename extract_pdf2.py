from pypdf import PdfReader
import sys

pdf_path = "HUMAN RESOURCE MANUAL.pdf"

try:
    reader = PdfReader(pdf_path)
    print(f"Total pages: {len(reader.pages)}\n")
    
    # Extract text from all pages and save to a file
    with open("manual_extracted.txt", "w", encoding="utf-8") as outfile:
        for i, page in enumerate(reader.pages, 1):
            text = page.extract_text()
            outfile.write(f"\n{'='*80}\n")
            outfile.write(f"PAGE {i}\n")
            outfile.write(f"{'='*80}\n\n")
            outfile.write(text)
            outfile.write("\n")
            
            # Also print first 50 pages to terminal
            if i <= 50:
                print(f"\n{'='*80}")
                print(f"PAGE {i}")
                print(f"{'='*80}\n")
                print(text)
    
    print(f"\n✓ Full extraction saved to manual_extracted.txt")
            
except Exception as e:
    print(f"Error extracting PDF: {e}", file=sys.stderr)
    import traceback
    traceback.print_exc()
