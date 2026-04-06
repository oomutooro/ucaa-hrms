import { FormEvent, useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import PageTitle from "../components/PageTitle";
import { Employee, ShiftAssignment } from "../types/models";

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
    <>
      <PageTitle title="Shift Scheduling" subtitle="Office and rotating roster management" />
      <div className="content-grid">
        <form className="card" onSubmit={onSubmit}>
          <h4>Assign Shift</h4>
          <select value={form.employeeId} onChange={(e) => setForm({ ...form, employeeId: e.target.value })}>
            <option value="">Unassigned</option>
            {employees.map((employee) => (
              <option key={employee.id} value={employee.id}>{employee.fullName}</option>
            ))}
          </select>
          <input type="date" value={form.shiftDate} onChange={(e) => setForm({ ...form, shiftDate: e.target.value })} required />
          <select value={form.shiftType} onChange={(e) => setForm({ ...form, shiftType: Number(e.target.value) })}>
            <option value={1}>Day</option>
            <option value={2}>Night</option>
            <option value={3}>Off</option>
          </select>
          <button type="submit">Assign</button>
        </form>

        <div className="card wide">
          <h4>Assignments</h4>
          <table>
            <thead>
              <tr>
                <th>Date</th>
                <th>Shift</th>
                <th>Employee</th>
                <th>Code</th>
              </tr>
            </thead>
            <tbody>
              {items.map((item) => (
                <tr key={item.id}>
                  <td>{item.shiftDate}</td>
                  <td>{item.shiftType}</td>
                  <td>{item.employeeName ?? "-"}</td>
                  <td>{item.shiftCode}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </>
  );
}
