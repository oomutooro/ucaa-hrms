import { FormEvent, useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import { Employee, ShiftAssignment } from "../types/models";

const shiftLabel: Record<number, string> = { 1: "Day", 2: "Night", 3: "Off" };

export default function ShiftsPage() {
  const [items, setItems] = useState<ShiftAssignment[]>([]);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [form, setForm] = useState({ employeeId: "", shiftDate: "", shiftType: 1 });

  const load = () => {
    apiClient.get<ShiftAssignment[]>("/shifts").then((r) => setItems(r.data));
    apiClient.get<Employee[]>("/employees").then((r) => setEmployees(r.data));
  };

  useEffect(() => {
    load();
  }, []);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    await apiClient.post("/shifts/assign", { ...form, shiftType: Number(form.shiftType), employeeId: form.employeeId || null });
    load();
  };

  return (
    <div>
      <div className="page-title-block">
        <h2>Shift Scheduling</h2>
        <p>Office and rotating roster management</p>
      </div>
      <div className="content-grid">
        <form className="card" onSubmit={onSubmit} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
          <div className="card-header" style={{ marginBottom: 4 }}>
            <span className="card-title">Assign Shift</span>
          </div>
          <div className="form-group">
            <label>Employee</label>
            <select className="form-control" value={form.employeeId} onChange={(e) => setForm({ ...form, employeeId: e.target.value })}>
              <option value="">Unassigned</option>
              {employees.map((employee) => (
                <option key={employee.id} value={employee.id}>{employee.fullName}</option>
              ))}
            </select>
          </div>
          <div className="form-group">
            <label>Shift Date</label>
            <input className="form-control" type="date" value={form.shiftDate} onChange={(e) => setForm({ ...form, shiftDate: e.target.value })} required />
          </div>
          <div className="form-group">
            <label>Shift Type</label>
            <select className="form-control" value={form.shiftType} onChange={(e) => setForm({ ...form, shiftType: Number(e.target.value) })}>
              <option value={1}>Day</option>
              <option value={2}>Night</option>
              <option value={3}>Off</option>
            </select>
          </div>
          <button type="submit" className="btn btn-primary">Assign Shift</button>
        </form>

        <div className="card wide">
          <div className="card-header">
            <span className="card-title">Shift Assignments</span>
          </div>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Date</th>
                  <th>Shift Type</th>
                  <th>Employee</th>
                  <th>Code</th>
                </tr>
              </thead>
              <tbody>
                {items.length === 0 ? (
                  <tr><td colSpan={4} style={{ textAlign: "center", color: "var(--text-muted)", padding: 24 }}>No shift assignments</td></tr>
                ) : items.map((item) => (
                  <tr key={item.id}>
                    <td>{item.shiftDate}</td>
                    <td>
                      <span style={{
                        background: item.shiftType === 1 ? "#dbeafe" : item.shiftType === 2 ? "#e0e7ff" : "#f3f4f6",
                        color: item.shiftType === 1 ? "var(--blue)" : item.shiftType === 2 ? "#4338ca" : "var(--text-muted)",
                        padding: "3px 10px", borderRadius: 50, fontSize: 12, fontWeight: 600
                      }}>{shiftLabel[item.shiftType] ?? item.shiftType}</span>
                    </td>
                    <td>{item.employeeName ?? "-"}</td>
                    <td style={{ fontFamily: "monospace", fontSize: 12 }}>{item.shiftCode}</td>
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
