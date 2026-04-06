import { useEffect, useState } from "react";
import apiClient from "../api/apiClient";
import {
  EmployeeOnboarding,
  OnboardingDetail,
  OnboardingItem,
  OnboardingTemplate,
  OnboardingTemplateTask,
} from "../types/models";

const STATUS_LABELS: Record<number, string> = { 1: "Not Started", 2: "In Progress", 3: "Completed" };
const STATUS_COLORS: Record<number, string> = {
  1: "#8b8b8b",
  2: "#f5a623",
  3: "#2dce89",
};

function Badge({ status }: { status: number }) {
  return (
    <span
      style={{
        display: "inline-block",
        padding: "2px 10px",
        borderRadius: 12,
        fontSize: 12,
        fontWeight: 600,
        background: STATUS_COLORS[status] + "22",
        color: STATUS_COLORS[status],
        border: `1px solid ${STATUS_COLORS[status]}44`,
      }}
    >
      {STATUS_LABELS[status] ?? "Unknown"}
    </span>
  );
}

function ProgressBar({ completed, total }: { completed: number; total: number }) {
  const pct = total === 0 ? 0 : Math.round((completed / total) * 100);
  return (
    <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
      <div style={{ flex: 1, background: "#e9ecef", borderRadius: 4, height: 8, overflow: "hidden" }}>
        <div
          style={{
            width: `${pct}%`,
            height: "100%",
            background: pct === 100 ? "#2dce89" : "#f5a623",
            borderRadius: 4,
            transition: "width 0.3s",
          }}
        />
      </div>
      <span style={{ fontSize: 12, color: "#666", minWidth: 40 }}>
        {completed}/{total}
      </span>
    </div>
  );
}

// ─── Templates Tab ────────────────────────────────────────────────────────────

interface TemplateDetailPanelProps {
  template: OnboardingTemplate;
  onClose: () => void;
  onDeleted: () => void;
}

function TemplateDetailPanel({ template, onClose, onDeleted }: TemplateDetailPanelProps) {
  const [tasks, setTasks] = useState<OnboardingTemplateTask[]>([]);
  const [loading, setLoading] = useState(true);
  const [showAddTask, setShowAddTask] = useState(false);
  const [taskForm, setTaskForm] = useState({ title: "", category: "HR Admin", isRequired: true, sortOrder: 0 });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const CATEGORIES = ["HR Admin", "IT Setup", "Documents", "Orientation", "Health & Safety", "Other"];

  useEffect(() => {
    apiClient.get<OnboardingTemplateTask[]>(`/onboarding/templates/${template.id}/tasks`)
      .then(r => setTasks(r.data))
      .catch(() => {})
      .finally(() => setLoading(false));
  }, [template.id]);

  const addTask = async () => {
    if (!taskForm.title.trim()) { setError("Task title is required."); return; }
    setSaving(true); setError("");
    try {
      const r = await apiClient.post<OnboardingTemplateTask>(`/onboarding/templates/${template.id}/tasks`, taskForm);
      setTasks(prev => [...prev, r.data]);
      setTaskForm({ title: "", category: "HR Admin", isRequired: true, sortOrder: 0 });
      setShowAddTask(false);
    } catch (e: any) {
      setError(e.response?.data?.message ?? "Failed to add task.");
    } finally { setSaving(false); }
  };

  const deleteTask = async (taskId: string) => {
    if (!window.confirm("Remove this task?")) return;
    try {
      await apiClient.delete(`/onboarding/templates/${template.id}/tasks/${taskId}`);
      setTasks(prev => prev.filter(t => t.id !== taskId));
    } catch { setError("Failed to remove task."); }
  };

  const deleteTemplate = async () => {
    if (!window.confirm(`Delete template "${template.name}"? This cannot be undone.`)) return;
    try {
      await apiClient.delete(`/onboarding/templates/${template.id}`);
      onDeleted();
    } catch (e: any) {
      setError(e.response?.data?.message ?? "Failed to delete template.");
    }
  };

  return (
    <div className="card" style={{ marginTop: 16 }}>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
        <div>
          <h3 style={{ margin: 0, fontSize: 16 }}>{template.name}</h3>
          {template.description && <p style={{ margin: "4px 0 0", color: "#666", fontSize: 13 }}>{template.description}</p>}
        </div>
        <div style={{ display: "flex", gap: 8 }}>
          <button className="btn btn-outline" style={{ fontSize: 12 }} onClick={() => setShowAddTask(v => !v)}>
            + Add Task
          </button>
          <button
            className="btn"
            style={{ fontSize: 12, background: "#dc3545", borderColor: "#dc3545" }}
            onClick={deleteTemplate}
          >
            Delete Template
          </button>
          <button className="btn btn-outline" style={{ fontSize: 12 }} onClick={onClose}>Close</button>
        </div>
      </div>

      {error && <p style={{ color: "#dc3545", marginTop: 8, fontSize: 13 }}>{error}</p>}

      {showAddTask && (
        <div style={{ background: "#f8f9fa", borderRadius: 8, padding: 12, marginTop: 12 }}>
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr auto auto", gap: 8, alignItems: "end" }}>
            <div>
              <label className="form-label">Task Title</label>
              <input className="form-control" value={taskForm.title} onChange={e => setTaskForm(f => ({ ...f, title: e.target.value }))} placeholder="e.g. Submit ID documents" />
            </div>
            <div>
              <label className="form-label">Category</label>
              <select className="form-control" value={taskForm.category} onChange={e => setTaskForm(f => ({ ...f, category: e.target.value }))}>
                {CATEGORIES.map(c => <option key={c}>{c}</option>)}
              </select>
            </div>
            <div style={{ display: "flex", alignItems: "center", gap: 4, paddingBottom: 2 }}>
              <input type="checkbox" id="req-chk" checked={taskForm.isRequired} onChange={e => setTaskForm(f => ({ ...f, isRequired: e.target.checked }))} />
              <label htmlFor="req-chk" style={{ fontSize: 12, whiteSpace: "nowrap" }}>Required</label>
            </div>
            <button className="btn" style={{ fontSize: 12 }} onClick={addTask} disabled={saving}>
              {saving ? "Saving..." : "Add"}
            </button>
          </div>
        </div>
      )}

      <div style={{ marginTop: 12 }}>
        {loading ? <p style={{ color: "#aaa", fontSize: 13 }}>Loading tasks…</p> : tasks.length === 0 ? (
          <p style={{ color: "#aaa", fontSize: 13 }}>No tasks yet. Click "+ Add Task" to begin.</p>
        ) : (
          <table className="table">
            <thead>
              <tr>
                <th>#</th>
                <th>Task</th>
                <th>Category</th>
                <th>Required</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {[...tasks].sort((a, b) => a.sortOrder - b.sortOrder).map((t, i) => (
                <tr key={t.id}>
                  <td style={{ width: 40, color: "#aaa", fontSize: 12 }}>{i + 1}</td>
                  <td>{t.title}</td>
                  <td><span style={{ fontSize: 12, background: "#e3eaff", color: "#3b5bdb", padding: "2px 8px", borderRadius: 8 }}>{t.category}</span></td>
                  <td>{t.isRequired ? <span style={{ color: "#2dce89" }}>✓</span> : <span style={{ color: "#aaa" }}>—</span>}</td>
                  <td>
                    <button onClick={() => deleteTask(t.id)} style={{ background: "none", border: "none", cursor: "pointer", color: "#dc3545", fontSize: 13 }}>✕</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
}

function TemplatesTab() {
  const [templates, setTemplates] = useState<OnboardingTemplate[]>([]);
  const [loading, setLoading] = useState(true);
  const [selected, setSelected] = useState<OnboardingTemplate | null>(null);
  const [showCreate, setShowCreate] = useState(false);
  const [form, setForm] = useState({ name: "", description: "" });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const load = () => {
    setLoading(true);
    apiClient.get<OnboardingTemplate[]>("/onboarding/templates")
      .then(r => setTemplates(r.data))
      .catch(() => {})
      .finally(() => setLoading(false));
  };

  useEffect(load, []);

  const create = async () => {
    if (!form.name.trim()) { setError("Template name is required."); return; }
    setSaving(true); setError("");
    try {
      await apiClient.post("/onboarding/templates", form);
      setForm({ name: "", description: "" });
      setShowCreate(false);
      load();
    } catch (e: any) {
      setError(e.response?.data?.message ?? "Failed to create template.");
    } finally { setSaving(false); }
  };

  return (
    <div>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 16 }}>
        <p style={{ margin: 0, color: "#666", fontSize: 14 }}>
          Define reusable onboarding checklists that can be applied to new hires.
        </p>
        <button className="btn" onClick={() => setShowCreate(v => !v)}>+ New Template</button>
      </div>

      {showCreate && (
        <div className="card" style={{ marginBottom: 16 }}>
          <h4 style={{ margin: "0 0 12px" }}>New Onboarding Template</h4>
          {error && <p style={{ color: "#dc3545", fontSize: 13 }}>{error}</p>}
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12 }}>
            <div>
              <label className="form-label">Template Name *</label>
              <input className="form-control" value={form.name} onChange={e => setForm(f => ({ ...f, name: e.target.value }))} placeholder="e.g. Standard Officer Onboarding" />
            </div>
            <div>
              <label className="form-label">Description</label>
              <input className="form-control" value={form.description} onChange={e => setForm(f => ({ ...f, description: e.target.value }))} placeholder="Optional description" />
            </div>
          </div>
          <div style={{ marginTop: 12, display: "flex", gap: 8 }}>
            <button className="btn" onClick={create} disabled={saving}>{saving ? "Saving..." : "Create Template"}</button>
            <button className="btn btn-outline" onClick={() => { setShowCreate(false); setError(""); }}>Cancel</button>
          </div>
        </div>
      )}

      {loading ? <p style={{ color: "#aaa" }}>Loading…</p> : templates.length === 0 ? (
        <div style={{ textAlign: "center", padding: "40px 0", color: "#aaa" }}>
          <p>No onboarding templates yet.</p>
          <button className="btn" onClick={() => setShowCreate(true)}>Create First Template</button>
        </div>
      ) : (
        <div>
          <table className="table">
            <thead>
              <tr>
                <th>Template Name</th>
                <th>Description</th>
                <th>Tasks</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {templates.map(t => (
                <tr
                  key={t.id}
                  style={{ cursor: "pointer", background: selected?.id === t.id ? "#f0f4ff" : undefined }}
                  onClick={() => setSelected(prev => prev?.id === t.id ? null : t)}
                >
                  <td style={{ fontWeight: 500 }}>{t.name}</td>
                  <td style={{ color: "#666", fontSize: 13 }}>{t.description || "—"}</td>
                  <td>
                    <span style={{ background: "#e3eaff", color: "#3b5bdb", padding: "2px 8px", borderRadius: 8, fontSize: 12 }}>
                      {t.taskCount} task{t.taskCount !== 1 ? "s" : ""}
                    </span>
                  </td>
                  <td style={{ color: "#aaa", fontSize: 12 }}>{selected?.id === t.id ? "▲ collapse" : "▼ expand"}</td>
                </tr>
              ))}
            </tbody>
          </table>
          {selected && (
            <TemplateDetailPanel
              key={selected.id}
              template={selected}
              onClose={() => setSelected(null)}
              onDeleted={() => { setSelected(null); load(); }}
            />
          )}
        </div>
      )}
    </div>
  );
}

// ─── Onboardings Tab ──────────────────────────────────────────────────────────

interface OnboardingChecklistProps {
  record: EmployeeOnboarding;
  onClose: () => void;
  onUpdated: () => void;
}

function OnboardingChecklist({ record, onClose, onUpdated }: OnboardingChecklistProps) {
  const [detail, setDetail] = useState<OnboardingDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [toggling, setToggling] = useState<string | null>(null);
  const [error, setError] = useState("");

  const load = () => {
    setLoading(true);
    apiClient.get<OnboardingDetail>(`/onboarding/records/${record.id}`)
      .then(r => setDetail(r.data))
      .catch(() => setError("Failed to load checklist."))
      .finally(() => setLoading(false));
  };

  useEffect(load, [record.id]);

  const toggleItem = async (item: OnboardingItem) => {
    setToggling(item.id);
    try {
      await apiClient.patch(`/onboarding/items/${item.id}`, {
        isCompleted: !item.isCompleted,
        notes: item.notes,
      });
      load();
      onUpdated();
    } catch { setError("Failed to update item."); }
    finally { setToggling(null); }
  };

  const grouped = detail
    ? detail.items.reduce<Record<string, OnboardingItem[]>>((acc, item) => {
        (acc[item.category] ||= []).push(item);
        return acc;
      }, {})
    : {};

  return (
    <div className="card" style={{ marginTop: 16 }}>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start", marginBottom: 12 }}>
        <div>
          <h3 style={{ margin: 0, fontSize: 16 }}>{record.employeeName} — Onboarding Checklist</h3>
          <p style={{ margin: "4px 0 0", fontSize: 12, color: "#666" }}>
            Started: {new Date(record.startDate).toLocaleDateString()} | Target: {new Date(record.targetCompletionDate).toLocaleDateString()}
          </p>
          <div style={{ marginTop: 6 }}>
            <ProgressBar completed={detail?.items.filter(i => i.isCompleted).length ?? record.completedItems} total={detail?.items.length ?? record.totalItems} />
          </div>
        </div>
        <button className="btn btn-outline" style={{ fontSize: 12 }} onClick={onClose}>Close</button>
      </div>

      {error && <p style={{ color: "#dc3545", fontSize: 13 }}>{error}</p>}

      {loading ? <p style={{ color: "#aaa", fontSize: 13 }}>Loading checklist…</p> : !detail ? null : detail.items.length === 0 ? (
        <p style={{ color: "#aaa", fontSize: 13 }}>No checklist items. Items can be added by HR.</p>
      ) : (
        Object.entries(grouped).map(([category, items]) => (
          <div key={category} style={{ marginBottom: 16 }}>
            <div style={{ fontWeight: 600, fontSize: 13, color: "#3b5bdb", marginBottom: 6, textTransform: "uppercase", letterSpacing: "0.5px" }}>
              {category}
            </div>
            {[...items].sort((a, b) => a.sortOrder - b.sortOrder).map(item => (
              <div
                key={item.id}
                style={{
                  display: "flex",
                  alignItems: "center",
                  gap: 10,
                  padding: "8px 12px",
                  borderRadius: 6,
                  marginBottom: 4,
                  background: item.isCompleted ? "#f0fff4" : "#fff",
                  border: `1px solid ${item.isCompleted ? "#b2f2d2" : "#e9ecef"}`,
                  cursor: toggling === item.id ? "wait" : "pointer",
                  opacity: toggling === item.id ? 0.6 : 1,
                }}
                onClick={() => toggling === item.id ? undefined : toggleItem(item)}
              >
                <div
                  style={{
                    width: 18,
                    height: 18,
                    borderRadius: "50%",
                    border: `2px solid ${item.isCompleted ? "#2dce89" : "#ced4da"}`,
                    background: item.isCompleted ? "#2dce89" : "transparent",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    flexShrink: 0,
                    transition: "all 0.2s",
                  }}
                >
                  {item.isCompleted && <span style={{ color: "#fff", fontSize: 10, lineHeight: 1 }}>✓</span>}
                </div>
                <div style={{ flex: 1 }}>
                  <span style={{ fontSize: 14, textDecoration: item.isCompleted ? "line-through" : "none", color: item.isCompleted ? "#aaa" : "#1a1f36" }}>
                    {item.title}
                  </span>
                  {item.isRequired && !item.isCompleted && (
                    <span style={{ marginLeft: 8, fontSize: 11, color: "#f5a623", fontWeight: 600 }}>Required</span>
                  )}
                </div>
                {item.completedAt && (
                  <span style={{ fontSize: 11, color: "#aaa" }}>{new Date(item.completedAt).toLocaleDateString()}</span>
                )}
              </div>
            ))}
          </div>
        ))
      )}
    </div>
  );
}

function OnboardingsTab() {
  const [records, setRecords] = useState<EmployeeOnboarding[]>([]);
  const [templates, setTemplates] = useState<OnboardingTemplate[]>([]);
  const [employees, setEmployees] = useState<{ id: string; fullName: string }[]>([]);
  const [loading, setLoading] = useState(true);
  const [selected, setSelected] = useState<EmployeeOnboarding | null>(null);
  const [showCreate, setShowCreate] = useState(false);
  const [form, setForm] = useState({
    employeeId: "",
    templateId: "",
    startDate: new Date().toISOString().slice(0, 10),
    targetCompletionDate: "",
    notes: "",
  });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const loadRecords = () => {
    apiClient.get<EmployeeOnboarding[]>("/onboarding/records")
      .then(r => setRecords(r.data))
      .catch(() => {})
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    loadRecords();
    apiClient.get<OnboardingTemplate[]>("/onboarding/templates").then(r => setTemplates(r.data)).catch(() => {});
    apiClient.get<{ id: string; fullName: string }[]>("/employees").then(r => setEmployees(r.data)).catch(() => {});
  }, []);

  const create = async () => {
    if (!form.employeeId) { setError("Please select an employee."); return; }
    if (!form.targetCompletionDate) { setError("Target completion date is required."); return; }
    setSaving(true); setError("");
    try {
      await apiClient.post("/onboarding/records", {
        ...form,
        templateId: form.templateId || null,
      });
      setForm({ employeeId: "", templateId: "", startDate: new Date().toISOString().slice(0, 10), targetCompletionDate: "", notes: "" });
      setShowCreate(false);
      loadRecords();
    } catch (e: any) {
      setError(e.response?.data?.message ?? "Failed to create onboarding record.");
    } finally { setSaving(false); }
  };

  return (
    <div>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 16 }}>
        <p style={{ margin: 0, color: "#666", fontSize: 14 }}>
          Track onboarding progress for each new hire.
        </p>
        <button className="btn" onClick={() => setShowCreate(v => !v)}>+ New Onboarding</button>
      </div>

      {showCreate && (
        <div className="card" style={{ marginBottom: 16 }}>
          <h4 style={{ margin: "0 0 12px" }}>Start New Employee Onboarding</h4>
          {error && <p style={{ color: "#dc3545", fontSize: 13 }}>{error}</p>}
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr 1fr", gap: 12 }}>
            <div>
              <label className="form-label">Employee *</label>
              <select className="form-control" value={form.employeeId} onChange={e => setForm(f => ({ ...f, employeeId: e.target.value }))}>
                <option value="">— Select Employee —</option>
                {employees.map(e => <option key={e.id} value={e.id}>{e.fullName}</option>)}
              </select>
            </div>
            <div>
              <label className="form-label">Template (optional)</label>
              <select className="form-control" value={form.templateId} onChange={e => setForm(f => ({ ...f, templateId: e.target.value }))}>
                <option value="">— No template (blank) —</option>
                {templates.map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
              </select>
            </div>
            <div>
              <label className="form-label">Start Date *</label>
              <input type="date" className="form-control" value={form.startDate} onChange={e => setForm(f => ({ ...f, startDate: e.target.value }))} />
            </div>
            <div>
              <label className="form-label">Target Completion *</label>
              <input type="date" className="form-control" value={form.targetCompletionDate} onChange={e => setForm(f => ({ ...f, targetCompletionDate: e.target.value }))} />
            </div>
            <div style={{ gridColumn: "span 2" }}>
              <label className="form-label">Notes</label>
              <input className="form-control" value={form.notes} onChange={e => setForm(f => ({ ...f, notes: e.target.value }))} placeholder="Optional notes" />
            </div>
          </div>
          <div style={{ marginTop: 12, display: "flex", gap: 8 }}>
            <button className="btn" onClick={create} disabled={saving}>{saving ? "Saving..." : "Start Onboarding"}</button>
            <button className="btn btn-outline" onClick={() => { setShowCreate(false); setError(""); }}>Cancel</button>
          </div>
        </div>
      )}

      {loading ? <p style={{ color: "#aaa" }}>Loading…</p> : records.length === 0 ? (
        <div style={{ textAlign: "center", padding: "40px 0", color: "#aaa" }}>
          <p>No onboarding records yet.</p>
          <button className="btn" onClick={() => setShowCreate(true)}>Start First Onboarding</button>
        </div>
      ) : (
        <div>
          <table className="table">
            <thead>
              <tr>
                <th>Employee</th>
                <th>Start Date</th>
                <th>Target Completion</th>
                <th>Progress</th>
                <th>Status</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {records.map(r => (
                <tr
                  key={r.id}
                  style={{ cursor: "pointer", background: selected?.id === r.id ? "#f0f4ff" : undefined }}
                  onClick={() => setSelected(prev => prev?.id === r.id ? null : r)}
                >
                  <td style={{ fontWeight: 500 }}>{r.employeeName}</td>
                  <td style={{ fontSize: 13 }}>{new Date(r.startDate).toLocaleDateString()}</td>
                  <td style={{ fontSize: 13 }}>{new Date(r.targetCompletionDate).toLocaleDateString()}</td>
                  <td style={{ minWidth: 140 }}>
                    <ProgressBar completed={r.completedItems} total={r.totalItems} />
                  </td>
                  <td><Badge status={r.status} /></td>
                  <td style={{ color: "#aaa", fontSize: 12 }}>{selected?.id === r.id ? "▲ collapse" : "▼ checklist"}</td>
                </tr>
              ))}
            </tbody>
          </table>
          {selected && (
            <OnboardingChecklist
              key={selected.id}
              record={selected}
              onClose={() => setSelected(null)}
              onUpdated={() => {
                loadRecords();
                // refresh selected to reflect new completedItems count
                apiClient.get<EmployeeOnboarding[]>("/onboarding/records").then(r => {
                  setRecords(r.data);
                  const updated = r.data.find(x => x.id === selected.id);
                  if (updated) setSelected(updated);
                });
              }}
            />
          )}
        </div>
      )}
    </div>
  );
}

// ─── Page Shell ───────────────────────────────────────────────────────────────

export default function OnboardingPage() {
  const [tab, setTab] = useState<"templates" | "records">("templates");

  return (
    <div style={{ padding: "24px 32px" }}>
      <div style={{ marginBottom: 24 }}>
        <h1 style={{ fontSize: 24, fontWeight: 700, margin: 0 }}>Onboarding</h1>
        <p style={{ color: "#666", margin: "4px 0 0" }}>Manage onboarding templates and employee onboarding checklists.</p>
      </div>

      <div style={{ display: "flex", gap: 0, marginBottom: 24, borderBottom: "2px solid #e9ecef" }}>
        {(["templates", "records"] as const).map(t => (
          <button
            key={t}
            onClick={() => setTab(t)}
            style={{
              background: "none",
              border: "none",
              borderBottom: tab === t ? "2px solid #3b5bdb" : "2px solid transparent",
              marginBottom: -2,
              padding: "10px 24px",
              fontWeight: tab === t ? 600 : 400,
              color: tab === t ? "#3b5bdb" : "#666",
              cursor: "pointer",
              fontSize: 14,
              textTransform: "capitalize",
            }}
          >
            {t === "templates" ? "Onboarding Templates" : "Employee Onboardings"}
          </button>
        ))}
      </div>

      <div className="card">
        {tab === "templates" ? <TemplatesTab /> : <OnboardingsTab />}
      </div>
    </div>
  );
}
