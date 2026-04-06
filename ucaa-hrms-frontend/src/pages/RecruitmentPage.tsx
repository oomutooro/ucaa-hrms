import { FormEvent, useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import { JobApplication, JobRequisition, Position } from "../types/models";

type Tab = "requisitions" | "applications";

// Requisition status values (mirror backend enum)
const REQ_STATUS = { Draft: 1, Open: 2, Closed: 3, Cancelled: 4 };
// Application status values (mirror backend enum)
const APP_STATUS_OPTIONS = [
  { value: 1, label: "Received" },
  { value: 2, label: "Shortlisted" },
  { value: 3, label: "Interview Scheduled" },
  { value: 4, label: "Offered" },
  { value: 5, label: "Hired" },
  { value: 6, label: "Rejected" },
];

const statusColor = (label: string) => {
  switch (label) {
    case "Open": case "Shortlisted": case "Offered": return "var(--green, #22c55e)";
    case "Hired": return "var(--accent)";
    case "Draft": return "var(--muted)";
    case "Closed": case "Cancelled": case "Rejected": return "var(--red)";
    default: return "var(--muted)";
  }
};

const emptyReqForm = { positionId: "", departmentId: "", vacanciesRequested: "1", justification: "", closingDate: "" };
const emptyAppForm = { requisitionId: "", applicantName: "", applicantEmail: "", applicantPhone: "", isInternal: false, employeeId: "" };

export default function RecruitmentPage() {
  const [tab, setTab] = useState<Tab>("requisitions");
  const [error, setError] = useState<string | null>(null);

  const [positions, setPositions] = useState<Position[]>([]);
  const [requisitions, setRequisitions] = useState<JobRequisition[]>([]);
  const [applications, setApplications] = useState<JobApplication[]>([]);

  // Requisition form
  const [reqForm, setReqForm] = useState(emptyReqForm);

  // Application form
  const [appForm, setAppForm] = useState(emptyAppForm);

  // Status update modal
  const [statusModal, setStatusModal] = useState<{ type: "req" | "app"; id: string; currentStatus: number; label: string } | null>(null);
  const [newStatus, setNewStatus] = useState<number>(1);
  const [reviewNotes, setReviewNotes] = useState("");
  const [interviewDate, setInterviewDate] = useState("");

  // Drill-down
  const [selectedReqId, setSelectedReqId] = useState<string | null>(null);

  const loadAll = () => {
    setError(null);
    apiClient.get<Position[]>("/jobarchitecture/positions").then((r) => setPositions(r.data)).catch(() => {});
    apiClient.get<JobRequisition[]>("/recruitment/requisitions").then((r) => setRequisitions(r.data)).catch(() => setError("Failed to load requisitions."));
    apiClient.get<JobApplication[]>("/recruitment/applications").then((r) => setApplications(r.data)).catch(() => setError("Failed to load applications."));
  };

  useEffect(() => { loadAll(); }, []);

  // ── Requisition submit ────────────────────────────────────────────────────
  const onReqSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    try {
      await apiClient.post("/recruitment/requisitions", {
        positionId: reqForm.positionId,
        departmentId: reqForm.departmentId,
        vacanciesRequested: Number(reqForm.vacanciesRequested),
        justification: reqForm.justification,
        closingDate: reqForm.closingDate,
      });
      setReqForm(emptyReqForm);
      loadAll();
    } catch (err: any) {
      setError(err?.response?.data?.detail ?? err?.response?.data ?? "Failed to create requisition.");
    }
  };

  // ── Application submit ────────────────────────────────────────────────────
  const onAppSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    try {
      await apiClient.post("/recruitment/applications", {
        requisitionId: appForm.requisitionId,
        applicantName: appForm.applicantName,
        applicantEmail: appForm.applicantEmail,
        applicantPhone: appForm.applicantPhone,
        isInternal: appForm.isInternal,
        employeeId: appForm.isInternal && appForm.employeeId ? appForm.employeeId : null,
      });
      setAppForm(emptyAppForm);
      loadAll();
    } catch (err: any) {
      setError(err?.response?.data?.detail ?? err?.response?.data ?? "Failed to submit application.");
    }
  };

  // ── Status update ─────────────────────────────────────────────────────────
  const openStatusModal = (type: "req" | "app", id: string, currentStatus: number, label: string) => {
    setStatusModal({ type, id, currentStatus, label });
    setNewStatus(currentStatus);
    setReviewNotes("");
    setInterviewDate("");
  };

  const submitStatusUpdate = async () => {
    if (!statusModal) return;
    setError(null);
    try {
      if (statusModal.type === "req") {
        await apiClient.patch(`/recruitment/requisitions/${statusModal.id}/status`, { status: newStatus });
      } else {
        await apiClient.patch(`/recruitment/applications/${statusModal.id}/status`, {
          status: newStatus,
          reviewNotes: reviewNotes || null,
          interviewDate: interviewDate || null,
        });
      }
      setStatusModal(null);
      loadAll();
    } catch (err: any) {
      setError(err?.response?.data?.detail ?? err?.response?.data ?? "Failed to update status.");
      setStatusModal(null);
    }
  };

  // ── Delete ────────────────────────────────────────────────────────────────
  const deleteReq = async (id: string) => {
    if (!window.confirm("Delete this requisition?")) return;
    try {
      await apiClient.delete(`/recruitment/requisitions/${id}`);
      loadAll();
    } catch (err: any) {
      setError(err?.response?.data?.detail ?? err?.response?.data ?? "Cannot delete requisition.");
    }
  };

  const deleteApp = async (id: string) => {
    if (!window.confirm("Delete this application?")) return;
    try {
      await apiClient.delete(`/recruitment/applications/${id}`);
      loadAll();
    } catch (err: any) {
      setError(err?.response?.data?.detail ?? err?.response?.data ?? "Cannot delete application.");
    }
  };

  // ── Derived ───────────────────────────────────────────────────────────────
  const openReqs = requisitions.filter((r) => r.status === REQ_STATUS.Open);
  const displayedApps = selectedReqId
    ? applications.filter((a) => a.requisitionId === selectedReqId)
    : applications;

  // Position → departmentId lookup for auto-fill
  const positionById = new Map(positions.map((p) => [p.id, p]));

  const badge = (label: string) => (
    <span style={{
      background: `${statusColor(label)}22`,
      color: statusColor(label),
      borderRadius: 4,
      padding: "2px 8px",
      fontSize: 12,
      fontWeight: 600
    }}>{label}</span>
  );

  return (
    <div>
      <div className="page-title-block">
        <h2>Recruitment</h2>
        <p>Manage job requisitions and track applicants through the hiring pipeline</p>
      </div>

      {error && (
        <div className="card" style={{ marginBottom: 16, color: "var(--red)" }}>{error}</div>
      )}

      {/* Tab bar */}
      <div style={{ display: "flex", gap: 4, marginBottom: 20 }}>
        {(["requisitions", "applications"] as Tab[]).map((t) => (
          <button key={t} onClick={() => setTab(t)} className={tab === t ? "btn btn-primary" : "btn btn-secondary"} style={{ textTransform: "capitalize" }}>
            {t === "requisitions" ? `Requisitions (${requisitions.length})` : `Applications (${applications.length})`}
          </button>
        ))}
      </div>

      {/* ── Requisitions tab ────────────────────────────────────────────────── */}
      {tab === "requisitions" && (
        <div className="content-grid">
          <form className="card" onSubmit={onReqSubmit} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
            <div className="card-header" style={{ marginBottom: 4 }}>
              <span className="card-title">New Requisition</span>
            </div>
            <div className="form-group">
              <label>Position</label>
              <select className="form-control" value={reqForm.positionId}
                onChange={(e) => {
                  const pos = positionById.get(e.target.value);
                  setReqForm({ ...reqForm, positionId: e.target.value, departmentId: pos?.departmentId ?? reqForm.departmentId });
                }}
                required>
                <option value="">— Select position —</option>
                {positions.map((p) => (
                  <option key={p.id} value={p.id}>{p.title} ({p.jobGradeCode})</option>
                ))}
              </select>
              {positions.length === 0 && <small style={{ color: "var(--muted)" }}>No positions defined. Add positions in Job Architecture first.</small>}
            </div>
            {reqForm.departmentId && (
              <div className="form-group">
                <label>Department</label>
                <input className="form-control" readOnly value={positionById.get(reqForm.positionId)?.departmentName ?? ""} />
              </div>
            )}
            <div className="form-group">
              <label>Vacancies Requested</label>
              <input className="form-control" type="number" min="1" value={reqForm.vacanciesRequested} onChange={(e) => setReqForm({ ...reqForm, vacanciesRequested: e.target.value })} required />
            </div>
            <div className="form-group">
              <label>Justification</label>
              <textarea className="form-control" rows={3} placeholder="Reason for requisition" value={reqForm.justification} onChange={(e) => setReqForm({ ...reqForm, justification: e.target.value })} required />
            </div>
            <div className="form-group">
              <label>Closing Date</label>
              <input className="form-control" type="date" value={reqForm.closingDate} onChange={(e) => setReqForm({ ...reqForm, closingDate: e.target.value })} required />
            </div>
            <button className="btn btn-primary" type="submit">Create Requisition</button>
          </form>

          <div className="card">
            <div className="card-header" style={{ marginBottom: 12 }}>
              <span className="card-title">Requisitions ({requisitions.length})</span>
            </div>
            <table className="data-table">
              <thead>
                <tr>
                  <th>Ref #</th>
                  <th>Position</th>
                  <th>Department</th>
                  <th>Vacancies</th>
                  <th>Closing</th>
                  <th>Status</th>
                  <th>Apps</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {requisitions.length === 0 && (
                  <tr><td colSpan={8} style={{ textAlign: "center", color: "var(--muted)" }}>No requisitions yet.</td></tr>
                )}
                {requisitions.map((r) => (
                  <tr key={r.id}>
                    <td><strong>{r.requisitionNumber}</strong></td>
                    <td>{r.positionTitle}</td>
                    <td>{r.departmentName}</td>
                    <td style={{ textAlign: "center" }}>{r.vacanciesRequested}</td>
                    <td>{r.closingDate}</td>
                    <td>{badge(r.statusLabel)}</td>
                    <td style={{ textAlign: "center" }}>
                      <button className="btn btn-secondary" style={{ padding: "2px 10px", fontSize: 12 }}
                        onClick={() => { setSelectedReqId(r.id); setTab("applications"); }}>
                        {r.applicationCount}
                      </button>
                    </td>
                    <td style={{ display: "flex", gap: 6 }}>
                      <button className="btn btn-secondary" style={{ padding: "4px 10px", fontSize: 12 }}
                        onClick={() => openStatusModal("req", r.id, r.status, r.statusLabel)}>
                        Status
                      </button>
                      <button className="btn btn-secondary" style={{ padding: "4px 10px", fontSize: 12, color: "var(--red)" }}
                        onClick={() => deleteReq(r.id)}>Delete</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* ── Applications tab ─────────────────────────────────────────────────── */}
      {tab === "applications" && (
        <div className="content-grid">
          <form className="card" onSubmit={onAppSubmit} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
            <div className="card-header" style={{ marginBottom: 4 }}>
              <span className="card-title">Submit Application</span>
            </div>
            <div className="form-group">
              <label>Requisition (Open only)</label>
              <select className="form-control" value={appForm.requisitionId} onChange={(e) => setAppForm({ ...appForm, requisitionId: e.target.value })} required>
                <option value="">— Select requisition —</option>
                {openReqs.map((r) => (
                  <option key={r.id} value={r.id}>{r.requisitionNumber} — {r.positionTitle}</option>
                ))}
              </select>
              {openReqs.length === 0 && <small style={{ color: "var(--muted)" }}>No open requisitions. Open a requisition first.</small>}
            </div>
            <div className="form-group">
              <label>Applicant Name</label>
              <input className="form-control" placeholder="Full name" value={appForm.applicantName} onChange={(e) => setAppForm({ ...appForm, applicantName: e.target.value })} required />
            </div>
            <div className="form-group">
              <label>Email</label>
              <input className="form-control" type="email" placeholder="applicant@email.com" value={appForm.applicantEmail} onChange={(e) => setAppForm({ ...appForm, applicantEmail: e.target.value })} required />
            </div>
            <div className="form-group">
              <label>Phone</label>
              <input className="form-control" placeholder="+256 xxx xxx xxx" value={appForm.applicantPhone} onChange={(e) => setAppForm({ ...appForm, applicantPhone: e.target.value })} required />
            </div>
            <div className="form-group" style={{ display: "flex", alignItems: "center", gap: 10 }}>
              <input type="checkbox" id="isInternal" checked={appForm.isInternal} onChange={(e) => setAppForm({ ...appForm, isInternal: e.target.checked, employeeId: "" })} />
              <label htmlFor="isInternal" style={{ marginBottom: 0 }}>Internal applicant (existing employee)</label>
            </div>
            <button className="btn btn-primary" type="submit">Submit Application</button>
          </form>

          <div className="card">
            <div className="card-header" style={{ marginBottom: 8, display: "flex", alignItems: "center", gap: 12 }}>
              <span className="card-title">
                {selectedReqId
                  ? `Applications for ${requisitions.find((r) => r.id === selectedReqId)?.requisitionNumber ?? ""} (${displayedApps.length})`
                  : `All Applications (${applications.length})`}
              </span>
              {selectedReqId && (
                <button className="btn btn-secondary" style={{ padding: "3px 10px", fontSize: 12 }} onClick={() => setSelectedReqId(null)}>
                  Show All
                </button>
              )}
            </div>
            <table className="data-table">
              <thead>
                <tr>
                  <th>Applicant</th>
                  <th>Email</th>
                  <th>Requisition</th>
                  <th>Position</th>
                  <th>Type</th>
                  <th>Status</th>
                  <th>Interview</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {displayedApps.length === 0 && (
                  <tr><td colSpan={8} style={{ textAlign: "center", color: "var(--muted)" }}>No applications yet.</td></tr>
                )}
                {displayedApps.map((a) => (
                  <tr key={a.id}>
                    <td><strong>{a.applicantName}</strong></td>
                    <td>{a.applicantEmail}</td>
                    <td>{a.requisitionNumber}</td>
                    <td>{a.positionTitle}</td>
                    <td>{badge(a.isInternal ? "Internal" : "External")}</td>
                    <td>{badge(a.statusLabel)}</td>
                    <td>{a.interviewDate ?? "—"}</td>
                    <td style={{ display: "flex", gap: 6 }}>
                      <button className="btn btn-secondary" style={{ padding: "4px 10px", fontSize: 12 }}
                        onClick={() => openStatusModal("app", a.id, a.status, a.statusLabel)}>
                        Status
                      </button>
                      <button className="btn btn-secondary" style={{ padding: "4px 10px", fontSize: 12, color: "var(--red)" }}
                        onClick={() => deleteApp(a.id)}>Delete</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* ── Status update modal ───────────────────────────────────────────────── */}
      {statusModal && (
        <div style={{ position: "fixed", inset: 0, background: "rgba(0,0,0,0.45)", display: "flex", alignItems: "center", justifyContent: "center", zIndex: 1000 }}>
          <div className="card" style={{ width: 400, display: "flex", flexDirection: "column", gap: 14 }}>
            <div className="card-header" style={{ marginBottom: 4 }}>
              <span className="card-title">Update {statusModal.type === "req" ? "Requisition" : "Application"} Status</span>
            </div>
            <div className="form-group">
              <label>Current Status</label>
              <div>{badge(statusModal.label)}</div>
            </div>
            <div className="form-group">
              <label>New Status</label>
              {statusModal.type === "req" ? (
                <select className="form-control" value={newStatus} onChange={(e) => setNewStatus(Number(e.target.value))}>
                  <option value={1}>Draft</option>
                  <option value={2}>Open</option>
                  <option value={3}>Closed</option>
                  <option value={4}>Cancelled</option>
                </select>
              ) : (
                <select className="form-control" value={newStatus} onChange={(e) => setNewStatus(Number(e.target.value))}>
                  {APP_STATUS_OPTIONS.map((o) => (
                    <option key={o.value} value={o.value}>{o.label}</option>
                  ))}
                </select>
              )}
            </div>
            {statusModal.type === "app" && (
              <>
                {newStatus === 3 && (
                  <div className="form-group">
                    <label>Interview Date</label>
                    <input className="form-control" type="date" value={interviewDate} onChange={(e) => setInterviewDate(e.target.value)} />
                  </div>
                )}
                <div className="form-group">
                  <label>Review Notes (optional)</label>
                  <textarea className="form-control" rows={3} placeholder="Reviewer comments..." value={reviewNotes} onChange={(e) => setReviewNotes(e.target.value)} />
                </div>
              </>
            )}
            <div style={{ display: "flex", gap: 8 }}>
              <button className="btn btn-primary" onClick={submitStatusUpdate}>Save</button>
              <button className="btn btn-secondary" onClick={() => setStatusModal(null)}>Cancel</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
