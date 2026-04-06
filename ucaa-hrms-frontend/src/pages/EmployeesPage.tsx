import { FormEvent, useEffect, useMemo, useState } from "react";
import apiClient from "../api/apiClient";
import PageTitle from "../components/PageTitle";
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
    <>
      <PageTitle title="Employees" subtitle="Workforce records and profiles" />
      <div className="content-grid">
        <form className="card" onSubmit={onSubmit}>
          <h4>Add Employee</h4>
          <input placeholder="Full name" value={form.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })} required />
          <input placeholder="Email" type="email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} required />
          <input placeholder="Phone" value={form.phoneNumber} onChange={(e) => setForm({ ...form, phoneNumber: e.target.value })} required />
          <input placeholder="Employee ID" value={form.employeeId} onChange={(e) => setForm({ ...form, employeeId: e.target.value })} required />
          <input placeholder="Job title" value={form.jobTitle} onChange={(e) => setForm({ ...form, jobTitle: e.target.value })} required />
          <select value={form.departmentId} onChange={(e) => setForm({ ...form, departmentId: e.target.value })} required>
            <option value="">Select department</option>
            {departments.map((d) => (
              <option value={d.id} key={d.id}>{d.name}</option>
            ))}
          </select>
          <select value={form.employmentType} onChange={(e) => setForm({ ...form, employmentType: Number(e.target.value) })}>
            <option value={1}>Permanent</option>
            <option value={2}>Contract</option>
            <option value={3}>Internship</option>
            <option value={4}>Casual</option>
          </select>
          <button type="submit">Create</button>
        </form>

        <div className="card wide">
          <div className="table-header">
            <h4>Employee Directory</h4>
            <input placeholder="Search" value={search} onChange={(e) => setSearch(e.target.value)} />
          </div>
          <table>
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Employee ID</th>
                <th>Department</th>
                <th>Title</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((e) => (
                <tr key={e.id}>
                  <td>{e.fullName}</td>
                  <td>{e.email}</td>
                  <td>{e.employeeId}</td>
                  <td>{e.departmentName}</td>
                  <td>{e.jobTitle}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </>
  );
}
