import { FormEvent, useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import PageTitle from "../components/PageTitle";
import { Department } from "../types/models";

export default function DepartmentsPage() {
  const [departments, setDepartments] = useState<Department[]>([]);
  const [name, setName] = useState("");
  const [parentDepartmentId, setParentDepartmentId] = useState("");

  const load = () => apiClient.get<Department[]>("/departments").then((res) => setDepartments(res.data));

  useEffect(() => {
    load();
  }, []);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    await apiClient.post("/departments", { name, parentDepartmentId: parentDepartmentId || null });
    setName("");
    setParentDepartmentId("");
    load();
  };

  return (
    <>
      <PageTitle title="Departments" subtitle="Nested functional hierarchy" />
      <div className="content-grid">
        <form className="card" onSubmit={onSubmit}>
          <h4>Create Department</h4>
          <input placeholder="Department name" value={name} onChange={(e) => setName(e.target.value)} required />
          <select value={parentDepartmentId} onChange={(e) => setParentDepartmentId(e.target.value)}>
            <option value="">No parent</option>
            {departments.map((d) => (
              <option key={d.id} value={d.id}>{d.name}</option>
            ))}
          </select>
          <button type="submit">Save</button>
        </form>
        <div className="card wide">
          <h4>Structure</h4>
          <ul className="tree-list">
            {departments.map((d) => (
              <li key={d.id}>{d.name}{d.parentDepartmentId ? " (child)" : " (root)"}</li>
            ))}
          </ul>
        </div>
      </div>
    </>
  );
}
