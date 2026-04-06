import { FormEvent, useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import { DocumentItem } from "../types/models";

const docTypeLabel: Record<number, string> = { 1: "Contract", 2: "Job Description", 3: "Policy", 4: "Other" };

export default function DocumentsPage() {
  const [items, setItems] = useState<DocumentItem[]>([]);
  const [file, setFile] = useState<File | null>(null);
  const [documentType, setDocumentType] = useState(1);

  const load = () => apiClient.get<DocumentItem[]>("/documents").then((r) => setItems(r.data));

  useEffect(() => {
    load();
  }, []);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!file) {
      return;
    }

    const data = new FormData();
    data.append("file", file);
    data.append("documentType", String(documentType));

    await apiClient.post("/documents/upload", data, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    setFile(null);
    load();
  };

  return (
    <div>
      <div className="page-title-block">
        <h2>Document Management</h2>
        <p>Contracts, policies, and job descriptions</p>
      </div>
      <div className="content-grid">
        <form className="card" onSubmit={onSubmit} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
          <div className="card-header" style={{ marginBottom: 4 }}>
            <span className="card-title">Upload Document</span>
          </div>
          <div className="form-group">
            <label>File</label>
            <input
              className="form-control"
              type="file"
              style={{ padding: "8px 14px" }}
              onChange={(e) => setFile(e.target.files?.[0] ?? null)}
              required
            />
          </div>
          <div className="form-group">
            <label>Document Type</label>
            <select className="form-control" value={documentType} onChange={(e) => setDocumentType(Number(e.target.value))}>
              <option value={1}>Contract</option>
              <option value={2}>Job Description</option>
              <option value={3}>Policy</option>
              <option value={4}>Other</option>
            </select>
          </div>
          <button type="submit" className="btn btn-primary">Upload Document</button>
        </form>

        <div className="card wide">
          <div className="card-header">
            <span className="card-title">Stored Documents</span>
          </div>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>File Name</th>
                  <th>Type</th>
                  <th>Size</th>
                </tr>
              </thead>
              <tbody>
                {items.length === 0 ? (
                  <tr><td colSpan={3} style={{ textAlign: "center", color: "var(--text-muted)", padding: 24 }}>No documents uploaded</td></tr>
                ) : items.map((item) => (
                  <tr key={item.id}>
                    <td>
                      <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
                        <svg viewBox="0 0 24 24" fill="var(--blue)" width="16" height="16">
                          <path d="M14 2H6c-1.1 0-1.99.9-1.99 2L4 20c0 1.1.89 2 1.99 2H18c1.1 0 2-.9 2-2V8l-6-6zm2 16H8v-2h8v2zm0-4H8v-2h8v2zm-3-5V3.5L18.5 9H13z"/>
                        </svg>
                        {item.fileName}
                      </div>
                    </td>
                    <td>
                      <span style={{ background: "#dbeafe", color: "var(--blue)", padding: "3px 10px", borderRadius: 50, fontSize: 12, fontWeight: 600 }}>
                        {docTypeLabel[item.documentType] ?? item.documentType}
                      </span>
                    </td>
                    <td style={{ color: "var(--text-muted)", fontSize: 12 }}>
                      {item.fileSizeBytes < 1024 ? `${item.fileSizeBytes} B` : item.fileSizeBytes < 1048576 ? `${(item.fileSizeBytes / 1024).toFixed(1)} KB` : `${(item.fileSizeBytes / 1048576).toFixed(1)} MB`}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
}
