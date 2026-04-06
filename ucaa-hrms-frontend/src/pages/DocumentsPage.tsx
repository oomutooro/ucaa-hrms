import { FormEvent, useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import PageTitle from "../components/PageTitle";
import { DocumentItem } from "../types/models";

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
    <>
      <PageTitle title="Document Management" subtitle="Contracts, policies, and job descriptions" />
      <div className="content-grid">
        <form className="card" onSubmit={onSubmit}>
          <h4>Upload Document</h4>
          <input type="file" onChange={(e) => setFile(e.target.files?.[0] ?? null)} required />
          <select value={documentType} onChange={(e) => setDocumentType(Number(e.target.value))}>
            <option value={1}>Contract</option>
            <option value={2}>Job Description</option>
            <option value={3}>Policy</option>
            <option value={4}>Other</option>
          </select>
          <button type="submit">Upload</button>
        </form>

        <div className="card wide">
          <h4>Stored Documents</h4>
          <table>
            <thead>
              <tr>
                <th>File</th>
                <th>Type</th>
                <th>Size (bytes)</th>
              </tr>
            </thead>
            <tbody>
              {items.map((item) => (
                <tr key={item.id}>
                  <td>{item.fileName}</td>
                  <td>{item.documentType}</td>
                  <td>{item.fileSizeBytes}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </>
  );
}
