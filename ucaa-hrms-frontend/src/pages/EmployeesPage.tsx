import { FormEvent, useEffect, useMemo, useState } from "react";
import apiClient from "../api/apiClient";
import { Department, Employee } from "../types/models";

export default function EmployeesPage() {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [departments, setDepartments] = useState<Department[]>([]);
  const [search, setSearch] = useState("");

  const [form, setForm] = useState({
    fullName: "",
    email: "",
    phoneNumber: "",
    employeeId: "",
    departmentId: "",
    jobTitle: "",
    employmentType: 1,
  });

  const loadData = () => {
    apiClient.get<Employee[]>("/employees").then((res) => setEmployees(res.data));
    apiClient.get<Department[]>("/departments").then((res) => setDepartments(res.data));
  };

  useEffect(() => {
    loadData();
  }, []);

  const filtered = useMemo(
    () => employees.filter((e) => `${e.fullName} ${e.email} ${e.employeeId}`.toLowerCase().includes(search.toLowerCase())),
    [employees, search]
  );

  const onSubmit = async (event: FormEvent) => {
    event.preventDefault();
    await apiClient.post("/employees", {
      ...form,
      departmentId: form.departmentId,
      employmentType: Number(form.employmentType),
    });
    setForm({ fullName: "", email: "", phoneNumber: "", employeeId: "", departmentId: "", jobTitle: "", employmentType: 1 });
    loadData();
  };

  return (
    <div>
      <div className="page-title-block">
        <h2>Employee Management</h2>
        <p>Workforce records and profiles</p>
      </div>
      <div className="content-grid">
        <form className="card" onSubmit={onSubmit} style={{ display: "flex", flexDirection: "column", gap: 12 }}>
          <div className="card-header" style={{ marginBottom: 4 }}>
            <span className="card-title">Add Employee</span>
          </div>
          <div className="form-group">
            <label>Full Name</label>
            <input className="form-control" placeholder="Full name" value={form.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })} required />
          </div>
          <div className="form-group">
            <label>Email</label>
            <input className="form-control" placeholder="Email" type="email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} required />
          </div>
          <div className="form-group">
            <label>Phone</label>
            <input className="form-control" placeholder="Phone" value={form.phoneNumber} onChange={(e) => setForm({ ...form, phoneNumber: e.target.value })} required />
          </div>
          <div className="form-group">
            <label>Employee ID</label>
            <input className="form-control" placeholder="Employee ID" value={form.employeeId} onChange={(e) => setForm({ ...form, employeeId: e.target.value })} required />
          </div>
          <div className="form-group">
            <label>Job Title</label>
            <input className="form-control" placeholder="Job title" value={form.jobTitle} onChange={(e) => setForm({ ...form, jobTitle: e.target.value })} required />
          </div>
          <div className="form-group">
            <label>Department</label>
            <select className="form-control" value={form.departmentId} onChange={(e) => setForm({ ...form, departmentId: e.target.value })} required>
              <option value="">Select department</option>
              {departments.map((d) => (
                <option value={d.id} key={d.id}>{d.name}</option>
              ))}
            </select>
          </div>
          <div className="form-group">
            <label>Employment Type</label>
            <select className="form-control" value={form.employmentType} onChange={(e) => setForm({ ...form, employmentType: Number(e.target.value) })}>
              <option value={1}>Permanent</option>
              <option value={2}>Contract</option>
              <option value={3}>Internship</option>
              <option value={4}>Casual</option>
            </select>
          </div>
          <button type="submit" className="btn btn-primary">Create Employee</button>
        </form>

        <div className="card wide">
          <div className="card-header">
            <span className="card-title">Employee Directory</span>
            <input
              className="form-control"
              style={{ width: 200, marginBottom: 0 }}
              placeholder="Search..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Employee</th>
                  <th>Email</th>
                  <th>ID</th>
                  <th>Department</th>
                  <th>Title</th>
                  <th>Type</th>
                </tr>
              </thead>
              <tbody>
                {filtered.length === 0 ? (
                  <tr><td colSpan={6} style={{ textAlign: "center", color: "var(--text-muted)", padding: 24 }}>No employees found</td></tr>
                ) : filtered.map((e) => (
                  <tr key={e.id}>
                    <td>
                      <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
                        <div className="emp-avatar" style={{ width: 30, height: 30, fontSize: 12 }}>{e.fullName[0]}</div>
                        <span style={{ fontWeight: 600, fontSize: 13 }}>{e.fullName}</span>
                      </div>
                    </td>
                    <td>{e.email}</td>
                    <td>{e.employeeId}</td>
                    <td>{e.departmentName}</td>
                    <td>{e.jobTitle}</td>
                    <td>
                      <span style={{
                        background: "#f3f4f6", borderRadius: 50, padding: "2px 10px",
                        fontSize: 11, fontWeight: 600, color: "var(--text-muted)"
                      }}>
                        {["", "Permanent", "Contract", "Internship", "Casual"][e.employmentType] ?? ""}
                      </span>
                    </td>
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
