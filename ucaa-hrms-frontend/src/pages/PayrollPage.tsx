import { FormEvent, useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import { Employee, PayrollRecord } from "../types/models";

export default function PayrollPage() {
  const [records, setRecords] = useState<PayrollRecord[]>([]);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [form, setForm] = useState({ employeeId: "", basicSalary: 0, allowances: 0, deductions: 0, payPeriod: "", notes: "" });

  const load = () => {
    apiClient.get<PayrollRecord[]>("/payroll").then((r) => setRecords(r.data));
    apiClient.get<Employee[]>("/employees").then((r) => setEmployees(r.data));
  };

  useEffect(() => {
    load();
  }, []);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    await apiClient.post("/payroll", {
      ...form,
      basicSalary: Number(form.basicSalary),
      allowances: Number(form.allowances),
      deductions: Number(form.deductions),
    });
    load();
  };

  return (
    <div>
      <div className="page-title-block">
        <h2>Payroll Management</h2>
        <p>Salary, allowances, and deductions</p>
      </div>
      <div className="content-grid">
        <form className="card" onSubmit={onSubmit} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
          <div className="card-header" style={{ marginBottom: 4 }}>
            <span className="card-title">Create Payroll Record</span>
          </div>
          <div className="form-group">
            <label>Employee</label>
            <select className="form-control" value={form.employeeId} onChange={(e) => setForm({ ...form, employeeId: e.target.value })} required>
              <option value="">Select employee</option>
              {employees.map((employee) => (
                <option key={employee.id} value={employee.id}>{employee.fullName}</option>
              ))}
            </select>
          </div>
          <div className="form-group">
            <label>Basic Salary (UGX)</label>
            <input className="form-control" type="number" placeholder="0" value={form.basicSalary} onChange={(e) => setForm({ ...form, basicSalary: Number(e.target.value) })} />
          </div>
          <div className="form-group">
            <label>Allowances (UGX)</label>
            <input className="form-control" type="number" placeholder="0" value={form.allowances} onChange={(e) => setForm({ ...form, allowances: Number(e.target.value) })} />
          </div>
          <div className="form-group">
            <label>Deductions (UGX)</label>
            <input className="form-control" type="number" placeholder="0" value={form.deductions} onChange={(e) => setForm({ ...form, deductions: Number(e.target.value) })} />
          </div>
          <div className="form-group">
            <label>Pay Period</label>
            <input className="form-control" type="date" value={form.payPeriod} onChange={(e) => setForm({ ...form, payPeriod: e.target.value })} required />
          </div>
          <div className="form-group">
            <label>Notes</label>
            <textarea className="form-control" placeholder="Optional notes..." value={form.notes} onChange={(e) => setForm({ ...form, notes: e.target.value })} />
          </div>
          <button type="submit" className="btn btn-primary">Save Record</button>
        </form>

        <div className="card wide">
          <div className="card-header">
            <span className="card-title">Payroll Records</span>
          </div>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Employee</th>
                  <th>Period</th>
                  <th>Basic (UGX)</th>
                  <th>Allowances</th>
                  <th>Deductions</th>
                  <th>Net Pay</th>
                </tr>
              </thead>
              <tbody>
                {records.length === 0 ? (
                  <tr><td colSpan={6} style={{ textAlign: "center", color: "var(--text-muted)", padding: 24 }}>No payroll records found</td></tr>
                ) : records.map((record) => (
                  <tr key={record.id}>
                    <td>
                      <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
                        <div className="emp-avatar" style={{ width: 30, height: 30, fontSize: 12 }}>{record.employeeName[0]}</div>
                        {record.employeeName}
                      </div>
                    </td>
                    <td>{record.payPeriod}</td>
                    <td>{record.basicSalary.toLocaleString()}</td>
                    <td>{record.allowances.toLocaleString()}</td>
                    <td>{record.deductions.toLocaleString()}</td>
                    <td style={{ fontWeight: 700, color: "var(--green)" }}>{record.netPay.toLocaleString()}</td>
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
