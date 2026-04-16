import pdfplumber

pdf_path = "HUMAN RESOURCE MANUAL.pdf"

try:
    with pdfplumber.open(pdf_path) as pdf:
        print(f"Total pages: {len(pdf.pages)}\n")
        
        # Extract text from all pages
        for i, page in enumerate(pdf.pages, 1):
            text = page.extract_text()
            print(f"\n{'='*80}")
            print(f"PAGE {i}")
            print(f"{'='*80}\n")
            print(text)
            
except Exception as e:
    print(f"Error extracting PDF: {e}")
