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
    if (!parentDepartmentId) {
      return;
    }

    await apiClient.post("/departments", { name, parentDepartmentId: parentDepartmentId || null });
    setName("");
    setParentDepartmentId("");
    load();
  };

  const byId = new Map(departments.map((d) => [d.id, d]));
  const getLevel = (department: Department): number => {
    let level = 1;
    let current = department;
    while (current.parentDepartmentId) {
      level += 1;
      const parent = byId.get(current.parentDepartmentId);
      if (!parent) {
        break;
      }

      current = parent;
    }

    return level;
  };

  const parentCandidates = departments.filter((d) => getLevel(d) < 3);

  return (
    <div>
      <div className="page-title-block">
        <h2>Departments</h2>
        <p>Create departments and sections under existing directorates</p>
      </div>
      <div className="content-grid">
        <form className="card" onSubmit={onSubmit} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
          <div className="card-header" style={{ marginBottom: 4 }}>
            <span className="card-title">Create Department / Section</span>
          </div>
          <div className="form-group">
            <label>Name</label>
            <input className="form-control" placeholder="e.g. Finance" value={name} onChange={(e) => setName(e.target.value)} required />
          </div>
          <div className="form-group">
            <label>Parent (Required)</label>
            <select className="form-control" value={parentDepartmentId} onChange={(e) => setParentDepartmentId(e.target.value)} required>
              <option value="">Select directorate or department</option>
              {parentCandidates.map((d) => (
                <option key={d.id} value={d.id}>{d.name}</option>
              ))}
            </select>
          </div>
          <button type="submit" className="btn btn-primary" disabled={!parentDepartmentId}>Save</button>
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
                      {(() => {
                        const level = getLevel(d);
                        const label = level === 1 ? "Directorate" : level === 2 ? "Department" : "Section";
                        const isRoot = level === 1;
                        return (
                      <span style={{
                        background: isRoot ? "#dbeafe" : "#f3f4f6",
                        color: isRoot ? "var(--blue)" : "var(--text-muted)",
                        padding: "3px 10px", borderRadius: 50, fontSize: 12, fontWeight: 600
                      }}>{label}</span>
                        );
                      })()}
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
