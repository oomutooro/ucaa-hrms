import { FormEvent, useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import PageTitle from "../components/PageTitle";
import { Employee, LeaveRequest } from "../types/models";

export default function LeavePage() {
  const [items, setItems] = useState<LeaveRequest[]>([]);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [form, setForm] = useState({ employeeId: "", leaveType: 1, startDate: "", endDate: "", reason: "" });

  const load = () => {
    apiClient.get<LeaveRequest[]>("/leave").then((r) => setItems(r.data));
    apiClient.get<Employee[]>("/employees").then((r) => setEmployees(r.data));
  };

  useEffect(() => {
    load();
  }, []);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    await apiClient.post("/leave/apply", { ...form, leaveType: Number(form.leaveType) });
    setForm({ employeeId: "", leaveType: 1, startDate: "", endDate: "", reason: "" });
    load();
  };

  return (
    <>
      <PageTitle title="Leave Management" subtitle="Application, review, and balance control" />
      <div className="content-grid">
        <form className="card" onSubmit={onSubmit}>
          <h4>Apply Leave</h4>
          <select value={form.employeeId} onChange={(e) => setForm({ ...form, employeeId: e.target.value })} required>
            <option value="">Select employee</option>
            {employees.map((employee) => (
              <option key={employee.id} value={employee.id}>{employee.fullName}</option>
            ))}
          </select>
          <select value={form.leaveType} onChange={(e) => setForm({ ...form, leaveType: Number(e.target.value) })}>
            <option value={1}>Annual</option>
            <option value={2}>Sick</option>
            <option value={3}>Emergency</option>
          </select>
          <input type="date" value={form.startDate} onChange={(e) => setForm({ ...form, startDate: e.target.value })} required />
          <input type="date" value={form.endDate} onChange={(e) => setForm({ ...form, endDate: e.target.value })} required />
          <textarea placeholder="Reason" value={form.reason} onChange={(e) => setForm({ ...form, reason: e.target.value })} required />
          <button type="submit">Submit</button>
        </form>

        <div className="card wide">
          <h4>Requests</h4>
          <table>
            <thead>
              <tr>
                <th>Employee</th>
                <th>Type</th>
                <th>Status</th>
                <th>From</th>
                <th>To</th>
              </tr>
            </thead>
            <tbody>
              {items.map((item) => (
                <tr key={item.id}>
                  <td>{item.employeeName}</td>
                  <td>{item.leaveType}</td>
                  <td>{item.status}</td>
                  <td>{item.startDate}</td>
                  <td>{item.endDate}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </>
  );
}
