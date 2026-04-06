import { FormEvent, useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import { Department } from "../types/models";

export default function DepartmentsPage() {
  const [departments, setDepartments] = useState<Department[]>([]);
  const [name, setName] = useState("");
  const [parentDepartmentId, setParentDepartmentId] = useState("");

  const load = () => apiClient.get<Department[]>("/departments").then((res) => setDepartments(res.data));

  useEffect(() => {
    load();
  }, []);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    await apiClient.post("/departments", { name, parentDepartmentId: parentDepartmentId || null });
    setName("");
    setParentDepartmentId("");
    load();
  };

  return (
    <div>
      <div className="page-title-block">
        <h2>Departments</h2>
        <p>Nested functional hierarchy</p>
      </div>
      <div className="content-grid">
        <form className="card" onSubmit={onSubmit} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
          <div className="card-header" style={{ marginBottom: 4 }}>
            <span className="card-title">Create Department</span>
          </div>
          <div className="form-group">
            <label>Department Name</label>
            <input className="form-control" placeholder="e.g. Finance" value={name} onChange={(e) => setName(e.target.value)} required />
          </div>
          <div className="form-group">
            <label>Parent Department</label>
            <select className="form-control" value={parentDepartmentId} onChange={(e) => setParentDepartmentId(e.target.value)}>
              <option value="">No parent (root)</option>
              {departments.map((d) => (
                <option key={d.id} value={d.id}>{d.name}</option>
              ))}
            </select>
          </div>
          <button type="submit" className="btn btn-primary">Save Department</button>
        </form>
        <div className="card wide">
          <div className="card-header">
            <span className="card-title">Department Structure</span>
          </div>
          <div className="table-wrap">
            <table>
              <thead><tr><th>Name</th><th>Type</th><th>Parent</th></tr></thead>
              <tbody>
                {departments.length === 0 ? (
                  <tr><td colSpan={3} style={{ textAlign: "center", color: "var(--text-muted)", padding: 24 }}>No departments yet</td></tr>
                ) : departments.map((d) => (
                  <tr key={d.id}>
                    <td style={{ fontWeight: 600 }}>{d.name}</td>
                    <td>
                      <span style={{
                        background: d.parentDepartmentId ? "#f3f4f6" : "#dbeafe",
                        color: d.parentDepartmentId ? "var(--text-muted)" : "var(--blue)",
                        padding: "3px 10px", borderRadius: 50, fontSize: 12, fontWeight: 600
                      }}>{d.parentDepartmentId ? "Sub-department" : "Root"}</span>
                    </td>
                    <td>{departments.find(p => p.id === d.parentDepartmentId)?.name ?? "—"}</td>
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
