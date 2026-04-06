import { FormEvent, useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import { Employee, LeaveRequest } from "../types/models";

const leaveTypes = [
  { id: 1, name: "Annual Leave", days: 21 },
  { id: 2, name: "Sick Leave", days: 14 },
  { id: 3, name: "Emergency Leave", days: 5 },
  { id: 4, name: "Maternity Leave", days: 60 },
];

const leaveTypeLabel: Record<number, string> = { 1: "Annual", 2: "Sick", 3: "Emergency", 4: "Maternity" };
const leaveStatusLabel: Record<number, string> = { 0: "Pending", 1: "Approved", 2: "Declined" };

export default function LeavePage() {
  const [items, setItems] = useState<LeaveRequest[]>([]);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [form, setForm] = useState({ employeeId: "", leaveType: 1, startDate: "", endDate: "", reason: "" });
  const [showForm, setShowForm] = useState(false);

  const load = () => {
    apiClient.get<LeaveRequest[]>("/leave").then((r) => setItems(r.data)).catch(() => {});
    apiClient.get<Employee[]>("/employees").then((r) => setEmployees(r.data)).catch(() => {});
  };

  useEffect(() => { load(); }, []);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    await apiClient.post("/leave/apply", { ...form, leaveType: Number(form.leaveType) });
    setForm({ employeeId: "", leaveType: 1, startDate: "", endDate: "", reason: "" });
    setShowForm(false);
    load();
  };

  return (
    <div>
      <div className="page-title-block">
        <h2>Leave Management</h2>
        <p>Application, review, and balance control</p>
      </div>

      {/* Leave Type Cards */}
      <div className="leave-type-cards">
        {leaveTypes.map(lt => (
          <div className="leave-type-card" key={lt.id}>
            <div className="leave-type-num">{lt.days}</div>
            <div>
              <div className="leave-type-name">{lt.name}</div>
              <button className="leave-type-apply" onClick={() => { setForm(f => ({ ...f, leaveType: lt.id })); setShowForm(true); }}>Apply</button>
            </div>
          </div>
        ))}
      </div>

      {/* Apply Form */}
      {showForm && (
        <div className="form-card" style={{ marginBottom: 24 }}>
          <div className="form-title">Apply for Leave</div>
          <div className="form-subtitle">Fill in the details to submit your leave request</div>
          <form onSubmit={onSubmit}>
            <div className="form-row">
              <div className="form-group">
                <label>Employee</label>
                <select className="form-control" value={form.employeeId} onChange={e => setForm({ ...form, employeeId: e.target.value })} required>
                  <option value="">Select employee</option>
                  {employees.map(emp => <option key={emp.id} value={emp.id}>{emp.fullName}</option>)}
                </select>
              </div>
              <div className="form-group">
                <label>Leave Type</label>
                <select className="form-control" value={form.leaveType} onChange={e => setForm({ ...form, leaveType: Number(e.target.value) })}>
                  {leaveTypes.map(lt => <option key={lt.id} value={lt.id}>{lt.name}</option>)}
                </select>
              </div>
            </div>
            <div className="form-row">
              <div className="form-group">
                <label>Start Date</label>
                <input className="form-control" type="date" value={form.startDate} onChange={e => setForm({ ...form, startDate: e.target.value })} required />
              </div>
              <div className="form-group">
                <label>End Date</label>
                <input className="form-control" type="date" value={form.endDate} onChange={e => setForm({ ...form, endDate: e.target.value })} required />
              </div>
            </div>
            <div className="form-group">
              <label>Reason</label>
              <textarea className="form-control" placeholder="State your reason..." value={form.reason} onChange={e => setForm({ ...form, reason: e.target.value })} required />
            </div>
            <div style={{ display: "flex", gap: 12 }}>
              <button type="submit" className="btn btn-green">Submit Request</button>
              <button type="button" className="btn btn-outline-red" onClick={() => setShowForm(false)}>Cancel</button>
            </div>
          </form>
        </div>
      )}

      {/* Leave History Table */}
      <div className="card">
        <div className="card-header">
          <span className="card-title">Leave History</span>
          <button className="btn btn-sm btn-yellow" onClick={() => setShowForm(true)}>+ New Request</button>
        </div>
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Employee</th>
                <th>Type</th>
                <th>Start</th>
                <th>End</th>
                <th>Status</th>
                <th>Reason</th>
              </tr>
            </thead>
            <tbody>
              {items.length === 0 ? (
                <tr><td colSpan={6} style={{ textAlign: "center", color: "var(--text-muted)", padding: 24 }}>No leave requests found</td></tr>
              ) : items.map(item => (
                <tr key={item.id}>
                  <td>{item.employeeName}</td>
                  <td>{leaveTypeLabel[item.leaveType] ?? item.leaveType}</td>
                  <td>{item.startDate}</td>
                  <td>{item.endDate}</td>
                  <td>
                    <span style={{
                      background: item.status === 1 ? "#dcfce7" : item.status === 2 ? "#fee2e2" : "#fef9c3",
                      color: item.status === 1 ? "var(--green)" : item.status === 2 ? "var(--red)" : "#854d0e",
                      padding: "3px 10px", borderRadius: 50, fontSize: 12, fontWeight: 600
                    }}>{leaveStatusLabel[item.status] ?? "Pending"}</span>
                  </td>
                  <td style={{ maxWidth: 180, overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>{item.reason}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
