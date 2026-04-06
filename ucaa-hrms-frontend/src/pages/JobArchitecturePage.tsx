import { FormEvent, useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import { Department, JobDescription, JobGrade, Position } from "../types/models";

type Tab = "grades" | "job-descriptions" | "positions";

const emptyGradeForm = { gradeCode: "", gradeTitle: "", minSalary: "", maxSalary: "" };
const emptyJdForm = { title: "", purposeStatement: "", keyAccountabilities: "", qualifications: "", jobGradeId: "" };
const emptyPositionForm = { title: "", departmentId: "", jobDescriptionId: "", approvedHeadcount: "1" };

export default function JobArchitecturePage() {
  const [tab, setTab] = useState<Tab>("grades");
  const [error, setError] = useState<string | null>(null);

  // ── Grades ──────────────────────────────────────────────────────────────
  const [grades, setGrades] = useState<JobGrade[]>([]);
  const [gradeForm, setGradeForm] = useState(emptyGradeForm);
  const [editingGradeId, setEditingGradeId] = useState<string | null>(null);

  // ── Job Descriptions ─────────────────────────────────────────────────────
  const [jds, setJds] = useState<JobDescription[]>([]);
  const [jdForm, setJdForm] = useState(emptyJdForm);
  const [editingJdId, setEditingJdId] = useState<string | null>(null);

  // ── Positions ────────────────────────────────────────────────────────────
  const [positions, setPositions] = useState<Position[]>([]);
  const [departments, setDepartments] = useState<Department[]>([]);
  const [positionForm, setPositionForm] = useState(emptyPositionForm);
  const [editingPositionId, setEditingPositionId] = useState<string | null>(null);

  const loadAll = () => {
    setError(null);
    apiClient.get<JobGrade[]>("/jobarchitecture/grades").then((r) => setGrades(r.data)).catch(() => setError("Failed to load grades."));
    apiClient.get<JobDescription[]>("/jobarchitecture/job-descriptions").then((r) => setJds(r.data)).catch(() => setError("Failed to load job descriptions."));
    apiClient.get<Position[]>("/jobarchitecture/positions").then((r) => setPositions(r.data)).catch(() => setError("Failed to load positions."));
    apiClient.get<Department[]>("/departments").then((r) => setDepartments(r.data)).catch(() => {});
  };

  useEffect(() => { loadAll(); }, []);

  // ── Grade handlers ────────────────────────────────────────────────────────
  const onGradeSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    const payload = {
      gradeCode: gradeForm.gradeCode,
      gradeTitle: gradeForm.gradeTitle,
      minSalary: Number(gradeForm.minSalary),
      maxSalary: Number(gradeForm.maxSalary),
    };
    try {
      if (editingGradeId) {
        await apiClient.put(`/jobarchitecture/grades/${editingGradeId}`, payload);
      } else {
        await apiClient.post("/jobarchitecture/grades", payload);
      }
      setGradeForm(emptyGradeForm);
      setEditingGradeId(null);
      loadAll();
    } catch (err: any) {
      setError(err?.response?.data?.detail ?? err?.response?.data ?? "Failed to save grade.");
    }
  };

  const startEditGrade = (g: JobGrade) => {
    setEditingGradeId(g.id);
    setGradeForm({ gradeCode: g.gradeCode, gradeTitle: g.gradeTitle, minSalary: String(g.minSalary), maxSalary: String(g.maxSalary) });
  };

  const deleteGrade = async (id: string) => {
    if (!window.confirm("Delete this grade?")) return;
    try {
      await apiClient.delete(`/jobarchitecture/grades/${id}`);
      loadAll();
    } catch (err: any) {
      setError(err?.response?.data?.detail ?? err?.response?.data ?? "Cannot delete grade.");
    }
  };

  // ── Job Description handlers ───────────────────────────────────────────────
  const onJdSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    const payload = {
      title: jdForm.title,
      purposeStatement: jdForm.purposeStatement,
      keyAccountabilities: jdForm.keyAccountabilities,
      qualifications: jdForm.qualifications,
      jobGradeId: jdForm.jobGradeId,
    };
    try {
      if (editingJdId) {
        await apiClient.put(`/jobarchitecture/job-descriptions/${editingJdId}`, payload);
      } else {
        await apiClient.post("/jobarchitecture/job-descriptions", payload);
      }
      setJdForm(emptyJdForm);
      setEditingJdId(null);
      loadAll();
    } catch (err: any) {
      setError(err?.response?.data?.detail ?? err?.response?.data ?? "Failed to save job description.");
    }
  };

  const startEditJd = (j: JobDescription) => {
    setEditingJdId(j.id);
    setJdForm({ title: j.title, purposeStatement: j.purposeStatement, keyAccountabilities: j.keyAccountabilities, qualifications: j.qualifications, jobGradeId: j.jobGradeId });
  };

  const deleteJd = async (id: string) => {
    if (!window.confirm("Delete this job description?")) return;
    try {
      await apiClient.delete(`/jobarchitecture/job-descriptions/${id}`);
      loadAll();
    } catch (err: any) {
      setError(err?.response?.data?.detail ?? err?.response?.data ?? "Cannot delete job description.");
    }
  };

  // ── Position handlers ─────────────────────────────────────────────────────
  const onPositionSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    const payload = {
      title: positionForm.title,
      departmentId: positionForm.departmentId,
      jobDescriptionId: positionForm.jobDescriptionId,
      approvedHeadcount: Number(positionForm.approvedHeadcount),
    };
    try {
      if (editingPositionId) {
        await apiClient.put(`/jobarchitecture/positions/${editingPositionId}`, payload);
      } else {
        await apiClient.post("/jobarchitecture/positions", payload);
      }
      setPositionForm(emptyPositionForm);
      setEditingPositionId(null);
      loadAll();
    } catch (err: any) {
      setError(err?.response?.data?.detail ?? err?.response?.data ?? "Failed to save position.");
    }
  };

  const startEditPosition = (p: Position) => {
    setEditingPositionId(p.id);
    setPositionForm({ title: p.title, departmentId: p.departmentId, jobDescriptionId: p.jobDescriptionId, approvedHeadcount: String(p.approvedHeadcount) });
  };

  const deletePosition = async (id: string) => {
    if (!window.confirm("Delete this position?")) return;
    try {
      await apiClient.delete(`/jobarchitecture/positions/${id}`);
      loadAll();
    } catch (err: any) {
      setError(err?.response?.data?.detail ?? err?.response?.data ?? "Cannot delete position.");
    }
  };

  const cancelEdit = () => {
    setEditingGradeId(null);
    setGradeForm(emptyGradeForm);
    setEditingJdId(null);
    setJdForm(emptyJdForm);
    setEditingPositionId(null);
    setPositionForm(emptyPositionForm);
    setError(null);
  };

  const fmt = (n: number) => n.toLocaleString("en-UG", { style: "currency", currency: "UGX", maximumFractionDigits: 0 });

  return (
    <div>
      <div className="page-title-block">
        <h2>Job Architecture</h2>
        <p>Manage job grades, job descriptions, and approved positions</p>
      </div>

      {error && (
        <div className="card" style={{ marginBottom: 16, color: "var(--red)" }}>
          {error}
        </div>
      )}

      {/* Tab bar */}
      <div style={{ display: "flex", gap: 4, marginBottom: 20 }}>
        {(["grades", "job-descriptions", "positions"] as Tab[]).map((t) => (
          <button
            key={t}
            onClick={() => { setTab(t); cancelEdit(); }}
            className={tab === t ? "btn btn-primary" : "btn btn-secondary"}
            style={{ textTransform: "capitalize" }}
          >
            {t === "grades" ? "Job Grades" : t === "job-descriptions" ? "Job Descriptions" : "Positions"}
          </button>
        ))}
      </div>

      {/* ── Job Grades tab ─────────────────────────────────────────────────── */}
      {tab === "grades" && (
        <div className="content-grid">
          <form className="card" onSubmit={onGradeSubmit} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
            <div className="card-header" style={{ marginBottom: 4 }}>
              <span className="card-title">{editingGradeId ? "Edit Grade" : "Add Job Grade"}</span>
            </div>
            <div className="form-group">
              <label>Grade Code</label>
              <input className="form-control" placeholder="e.g. Grade 13" value={gradeForm.gradeCode} onChange={(e) => setGradeForm({ ...gradeForm, gradeCode: e.target.value })} required />
            </div>
            <div className="form-group">
              <label>Grade Title</label>
              <input className="form-control" placeholder="e.g. Executive Grade 1A" value={gradeForm.gradeTitle} onChange={(e) => setGradeForm({ ...gradeForm, gradeTitle: e.target.value })} required />
            </div>
            <div className="form-group">
              <label>Min Salary (UGX)</label>
              <input className="form-control" type="number" min="0" placeholder="Minimum salary" value={gradeForm.minSalary} onChange={(e) => setGradeForm({ ...gradeForm, minSalary: e.target.value })} required />
            </div>
            <div className="form-group">
              <label>Max Salary (UGX)</label>
              <input className="form-control" type="number" min="0" placeholder="Maximum salary" value={gradeForm.maxSalary} onChange={(e) => setGradeForm({ ...gradeForm, maxSalary: e.target.value })} required />
            </div>
            <div style={{ display: "flex", gap: 8 }}>
              <button className="btn btn-primary" type="submit">{editingGradeId ? "Update" : "Add Grade"}</button>
              {editingGradeId && <button className="btn btn-secondary" type="button" onClick={cancelEdit}>Cancel</button>}
            </div>
          </form>

          <div className="card">
            <div className="card-header" style={{ marginBottom: 12 }}>
              <span className="card-title">Job Grades ({grades.length})</span>
            </div>
            <table className="data-table">
              <thead>
                <tr>
                  <th>Grade Code</th>
                  <th>Title</th>
                  <th>Min Salary</th>
                  <th>Max Salary</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {grades.length === 0 && (
                  <tr><td colSpan={5} style={{ textAlign: "center", color: "var(--muted)" }}>No grades defined yet.</td></tr>
                )}
                {grades.map((g) => (
                  <tr key={g.id}>
                    <td><strong>{g.gradeCode}</strong></td>
                    <td>{g.gradeTitle}</td>
                    <td>{fmt(g.minSalary)}</td>
                    <td>{fmt(g.maxSalary)}</td>
                    <td style={{ display: "flex", gap: 6 }}>
                      <button className="btn btn-secondary" style={{ padding: "4px 10px", fontSize: 12 }} onClick={() => startEditGrade(g)}>Edit</button>
                      <button className="btn btn-secondary" style={{ padding: "4px 10px", fontSize: 12, color: "var(--red)" }} onClick={() => deleteGrade(g.id)}>Delete</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* ── Job Descriptions tab ───────────────────────────────────────────── */}
      {tab === "job-descriptions" && (
        <div className="content-grid">
          <form className="card" onSubmit={onJdSubmit} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
            <div className="card-header" style={{ marginBottom: 4 }}>
              <span className="card-title">{editingJdId ? "Edit Job Description" : "Add Job Description"}</span>
            </div>
            <div className="form-group">
              <label>Job Title</label>
              <input className="form-control" placeholder="e.g. Senior Human Resource Officer" value={jdForm.title} onChange={(e) => setJdForm({ ...jdForm, title: e.target.value })} required />
            </div>
            <div className="form-group">
              <label>Job Grade</label>
              <select className="form-control" value={jdForm.jobGradeId} onChange={(e) => setJdForm({ ...jdForm, jobGradeId: e.target.value })} required>
                <option value="">— Select a grade —</option>
                {grades.map((g) => (
                  <option key={g.id} value={g.id}>{g.gradeCode} — {g.gradeTitle}</option>
                ))}
              </select>
              {grades.length === 0 && <small style={{ color: "var(--muted)" }}>No grades available. Add job grades first.</small>}
            </div>
            <div className="form-group">
              <label>Purpose Statement</label>
              <textarea className="form-control" rows={3} placeholder="Overall purpose of this role" value={jdForm.purposeStatement} onChange={(e) => setJdForm({ ...jdForm, purposeStatement: e.target.value })} required />
            </div>
            <div className="form-group">
              <label>Key Accountabilities</label>
              <textarea className="form-control" rows={5} placeholder="List key responsibilities and deliverables" value={jdForm.keyAccountabilities} onChange={(e) => setJdForm({ ...jdForm, keyAccountabilities: e.target.value })} required />
            </div>
            <div className="form-group">
              <label>Qualifications & Requirements</label>
              <textarea className="form-control" rows={4} placeholder="Required education, experience, skills" value={jdForm.qualifications} onChange={(e) => setJdForm({ ...jdForm, qualifications: e.target.value })} required />
            </div>
            <div style={{ display: "flex", gap: 8 }}>
              <button className="btn btn-primary" type="submit">{editingJdId ? "Update" : "Add Job Description"}</button>
              {editingJdId && <button className="btn btn-secondary" type="button" onClick={cancelEdit}>Cancel</button>}
            </div>
          </form>

          <div className="card">
            <div className="card-header" style={{ marginBottom: 12 }}>
              <span className="card-title">Job Descriptions ({jds.length})</span>
            </div>
            <table className="data-table">
              <thead>
                <tr>
                  <th>Title</th>
                  <th>Grade</th>
                  <th>Purpose</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {jds.length === 0 && (
                  <tr><td colSpan={4} style={{ textAlign: "center", color: "var(--muted)" }}>No job descriptions defined yet.</td></tr>
                )}
                {jds.map((j) => (
                  <tr key={j.id}>
                    <td><strong>{j.title}</strong></td>
                    <td><span style={{ background: "var(--accent-pale)", color: "var(--accent)", borderRadius: 4, padding: "2px 7px", fontSize: 12 }}>{j.jobGradeCode}</span></td>
                    <td style={{ maxWidth: 260, overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>{j.purposeStatement}</td>
                    <td style={{ display: "flex", gap: 6 }}>
                      <button className="btn btn-secondary" style={{ padding: "4px 10px", fontSize: 12 }} onClick={() => startEditJd(j)}>Edit</button>
                      <button className="btn btn-secondary" style={{ padding: "4px 10px", fontSize: 12, color: "var(--red)" }} onClick={() => deleteJd(j.id)}>Delete</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* ── Positions tab ──────────────────────────────────────────────────── */}
      {tab === "positions" && (
        <div className="content-grid">
          <form className="card" onSubmit={onPositionSubmit} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
            <div className="card-header" style={{ marginBottom: 4 }}>
              <span className="card-title">{editingPositionId ? "Edit Position" : "Add Position"}</span>
            </div>
            <div className="form-group">
              <label>Position Title</label>
              <input className="form-control" placeholder="e.g. HR Officer — DHRA" value={positionForm.title} onChange={(e) => setPositionForm({ ...positionForm, title: e.target.value })} required />
            </div>
            <div className="form-group">
              <label>Department / Section</label>
              <select className="form-control" value={positionForm.departmentId} onChange={(e) => setPositionForm({ ...positionForm, departmentId: e.target.value })} required>
                <option value="">— Select department —</option>
                {departments.filter((d) => Boolean(d.parentDepartmentId)).map((d) => (
                  <option key={d.id} value={d.id}>{d.name}</option>
                ))}
              </select>
            </div>
            <div className="form-group">
              <label>Job Description</label>
              <select className="form-control" value={positionForm.jobDescriptionId} onChange={(e) => setPositionForm({ ...positionForm, jobDescriptionId: e.target.value })} required>
                <option value="">— Select job description —</option>
                {jds.map((j) => (
                  <option key={j.id} value={j.id}>{j.title} ({j.jobGradeCode})</option>
                ))}
              </select>
              {jds.length === 0 && <small style={{ color: "var(--muted)" }}>No job descriptions available. Add them first.</small>}
            </div>
            <div className="form-group">
              <label>Approved Headcount</label>
              <input className="form-control" type="number" min="1" value={positionForm.approvedHeadcount} onChange={(e) => setPositionForm({ ...positionForm, approvedHeadcount: e.target.value })} required />
            </div>
            <div style={{ display: "flex", gap: 8 }}>
              <button className="btn btn-primary" type="submit">{editingPositionId ? "Update" : "Add Position"}</button>
              {editingPositionId && <button className="btn btn-secondary" type="button" onClick={cancelEdit}>Cancel</button>}
            </div>
          </form>

          <div className="card">
            <div className="card-header" style={{ marginBottom: 12 }}>
              <span className="card-title">Approved Positions ({positions.length})</span>
            </div>
            <table className="data-table">
              <thead>
                <tr>
                  <th>Position Title</th>
                  <th>Department</th>
                  <th>Job Description</th>
                  <th>Grade</th>
                  <th>Headcount</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {positions.length === 0 && (
                  <tr><td colSpan={6} style={{ textAlign: "center", color: "var(--muted)" }}>No positions defined yet.</td></tr>
                )}
                {positions.map((p) => (
                  <tr key={p.id}>
                    <td><strong>{p.title}</strong></td>
                    <td>{p.departmentName}</td>
                    <td>{p.jobDescriptionTitle}</td>
                    <td><span style={{ background: "var(--accent-pale)", color: "var(--accent)", borderRadius: 4, padding: "2px 7px", fontSize: 12 }}>{p.jobGradeCode}</span></td>
                    <td style={{ textAlign: "center" }}>{p.approvedHeadcount}</td>
                    <td style={{ display: "flex", gap: 6 }}>
                      <button className="btn btn-secondary" style={{ padding: "4px 10px", fontSize: 12 }} onClick={() => startEditPosition(p)}>Edit</button>
                      <button className="btn btn-secondary" style={{ padding: "4px 10px", fontSize: 12, color: "var(--red)" }} onClick={() => deletePosition(p.id)}>Delete</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}
