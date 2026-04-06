import { FormEvent, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import apiClient from "../api/apiClient";
import { Department } from "../types/models";

export default function DepartmentsPage() {
  const { departmentId } = useParams();
  const navigate = useNavigate();
  const [departments, setDepartments] = useState<Department[]>([]);
  const [name, setName] = useState("");
  const [loadError, setLoadError] = useState<string | null>(null);

  const load = () => apiClient
    .get<Department[]>("/departments")
    .then((res) => {
      setDepartments(res.data);
      setLoadError(null);
    })
    .catch(() => {
      setDepartments([]);
      setLoadError("Failed to load directorates/departments. Please sign in again and refresh.");
    });

  useEffect(() => {
    load();
  }, []);

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

  const currentNode = departmentId ? byId.get(departmentId) : undefined;
  const currentLevel = currentNode ? getLevel(currentNode) : 0;
  const children = currentNode
    ? departments.filter((d) => d.parentDepartmentId === currentNode.id)
    : departments.filter((d) => !d.parentDepartmentId);

  const breadcrumb: Department[] = [];
  if (currentNode) {
    let cursor: Department | undefined = currentNode;
    while (cursor) {
      breadcrumb.unshift(cursor);
      cursor = cursor.parentDepartmentId ? byId.get(cursor.parentDepartmentId) : undefined;
    }
  }

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!currentNode) {
      return;
    }

    await apiClient.post("/departments", { name, parentDepartmentId: currentNode.id });
    setName("");
    await load();
  };

  const childTypeLabel = currentLevel === 1 ? "Department" : "Section";
  const listingTypeLabel = currentLevel === 0 ? "Directorates" : currentLevel === 1 ? "Departments" : "Sections";
  const canCreateChild = Boolean(currentNode) && currentLevel < 3;

  return (
    <div>
      <div className="page-title-block">
        <h2>Organization Structure</h2>
        <p>Click through directorates, then create departments and sections in context.</p>
      </div>
      {loadError && (
        <div className="card" style={{ marginBottom: 16, color: "var(--red)" }}>
          {loadError}
        </div>
      )}

      <div className="card" style={{ marginBottom: 16 }}>
        <div style={{ display: "flex", gap: 8, flexWrap: "wrap", alignItems: "center", fontSize: 13 }}>
          <Link to="/departments" className="btn btn-sm" style={{ background: "#eef2ff", color: "var(--blue)", textDecoration: "none" }}>
            All Directorates
          </Link>
          {breadcrumb.map((item, index) => (
            <span key={item.id} style={{ display: "inline-flex", alignItems: "center", gap: 8 }}>
              <span style={{ color: "var(--text-muted)" }}>/</span>
              <button
                type="button"
                className="btn btn-sm"
                onClick={() => navigate(`/departments/${item.id}`)}
                style={{ background: index === breadcrumb.length - 1 ? "#dbeafe" : "#f3f4f6", color: "var(--text-main)" }}
              >
                {item.name}
              </button>
            </span>
          ))}
        </div>
      </div>

      <div className="content-grid">
        <div className="card" style={{ display: "flex", flexDirection: "column", gap: 12 }}>
          <div className="card-header" style={{ marginBottom: 4 }}>
            <span className="card-title">Create {childTypeLabel}</span>
          </div>
          {!currentNode ? (
            <div className="empty-state">Select a directorate first to create a department.</div>
          ) : currentLevel >= 3 ? (
            <div className="empty-state">You are on a section. Sections cannot have child sections.</div>
          ) : (
            <form onSubmit={onSubmit} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
              <div className="form-group">
                <label>Parent</label>
                <input className="form-control" value={currentNode.name} disabled />
              </div>
              <div className="form-group">
                <label>{childTypeLabel} Name</label>
                <input
                  className="form-control"
                  placeholder={currentLevel === 1 ? "e.g. Finance" : "e.g. Revenue Billing"}
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  required
                />
              </div>
              <button type="submit" className="btn btn-primary" disabled={!canCreateChild}>Save {childTypeLabel}</button>
            </form>
          )}
        </div>

        <div className="card wide">
          <div className="card-header">
            <span className="card-title">{listingTypeLabel}</span>
          </div>
          <div className="table-wrap">
            <table>
              <thead><tr><th>Name</th><th>Type</th><th>Parent</th></tr></thead>
              <tbody>
                {children.length === 0 ? (
                  <tr>
                    <td colSpan={3} style={{ textAlign: "center", color: "var(--text-muted)", padding: 24 }}>
                      No {listingTypeLabel.toLowerCase()} found.
                    </td>
                  </tr>
                ) : children.map((d) => (
                  <tr key={d.id}>
                    <td style={{ fontWeight: 600 }}>
                      <button
                        type="button"
                        className="btn btn-sm"
                        onClick={() => navigate(`/departments/${d.id}`)}
                        style={{ background: "transparent", color: "var(--blue)", padding: 0, fontWeight: 700 }}
                      >
                        {d.name}
                      </button>
                    </td>
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
