from pypdf import PdfReader

pdf_path = "HUMAN RESOURCE MANUAL.pdf"

try:
    reader = PdfReader(pdf_path)
    pages_with_content = []
    
    for i, page in enumerate(reader.pages):
        text = page.extract_text()
        if text and len(text.strip()) > 50:  # Only pages with substantial content
            pages_with_content.append((i+1, text))
    
    print(f"Found {len(pages_with_content)} pages with substantial content out of {len(reader.pages)} total pages\n")
    
    # Write content pages to file
    with open("manual_content_only.txt", "w", encoding="utf-8") as outfile:
        for page_num, text in pages_with_content:
            outfile.write(f"\n{'='*80}\nPAGE {page_num}\n{'='*80}\n\n")
            outfile.write(text)
            outfile.write("\n")
            
            # Print to console
            print(f"\n{'='*80}")
            print(f"PAGE {page_num}")
            print(f"{'='*80}\n")
            print(text[:1500])  # First 1500 chars
            print("\n[... continues ...]")
    
    print(f"\n✓ Content saved to manual_content_only.txt ({len(pages_with_content)} pages)")
    
except Exception as e:
    print(f"Error: {e}")
    import traceback
    traceback.print_exc()
