import { FormEvent, useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import PageTitle from "../components/PageTitle";
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
    <>
      <PageTitle title="Payroll" subtitle="Salary, allowances, and deductions" />
      <div className="content-grid">
        <form className="card" onSubmit={onSubmit}>
          <h4>Create Payroll Record</h4>
          <select value={form.employeeId} onChange={(e) => setForm({ ...form, employeeId: e.target.value })} required>
            <option value="">Select employee</option>
            {employees.map((employee) => (
              <option key={employee.id} value={employee.id}>{employee.fullName}</option>
            ))}
          </select>
          <input type="number" placeholder="Basic salary" value={form.basicSalary} onChange={(e) => setForm({ ...form, basicSalary: Number(e.target.value) })} />
          <input type="number" placeholder="Allowances" value={form.allowances} onChange={(e) => setForm({ ...form, allowances: Number(e.target.value) })} />
          <input type="number" placeholder="Deductions" value={form.deductions} onChange={(e) => setForm({ ...form, deductions: Number(e.target.value) })} />
          <input type="date" value={form.payPeriod} onChange={(e) => setForm({ ...form, payPeriod: e.target.value })} required />
          <textarea placeholder="Notes" value={form.notes} onChange={(e) => setForm({ ...form, notes: e.target.value })} />
          <button type="submit">Save</button>
        </form>

        <div className="card wide">
          <h4>Payroll Records</h4>
          <table>
            <thead>
              <tr>
                <th>Employee</th>
                <th>Period</th>
                <th>Basic</th>
                <th>Allow.</th>
                <th>Ded.</th>
                <th>Net</th>
              </tr>
            </thead>
            <tbody>
              {records.map((record) => (
                <tr key={record.id}>
                  <td>{record.employeeName}</td>
                  <td>{record.payPeriod}</td>
                  <td>{record.basicSalary}</td>
                  <td>{record.allowances}</td>
                  <td>{record.deductions}</td>
                  <td>{record.netPay}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </>
  );
}
