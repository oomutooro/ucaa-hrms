import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { AuthProvider } from "./auth/AuthContext";
import Layout from "./components/Layout";
import ProtectedRoute from "./components/ProtectedRoute";
import DashboardPage from "./pages/DashboardPage";
import DepartmentsPage from "./pages/DepartmentsPage";
import DocumentsPage from "./pages/DocumentsPage";
import EmployeesPage from "./pages/EmployeesPage";
import EmployeeProfilePage from "./pages/EmployeeProfilePage";
import BenefitsPage from "./pages/BenefitsPage";
import JobArchitecturePage from "./pages/JobArchitecturePage";
import LeavePage from "./pages/LeavePage";
import RecruitmentPage from "./pages/RecruitmentPage";
import OnboardingPage from "./pages/OnboardingPage";
import LoginPage from "./pages/LoginPage";
import NotFoundPage from "./pages/NotFoundPage";
import PayrollPage from "./pages/PayrollPage";
import ShiftsPage from "./pages/ShiftsPage";
import "./App.css";

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <Layout />
              </ProtectedRoute>
            }
          >
            <Route index element={<Navigate to="/dashboard" replace />} />
            <Route path="dashboard" element={<DashboardPage />} />
            <Route path="employees" element={<EmployeesPage />} />
            <Route path="employees/:id" element={<EmployeeProfilePage />} />
            <Route path="departments" element={<DepartmentsPage />} />
            <Route path="departments/:departmentId" element={<DepartmentsPage />} />
            <Route path="job-architecture" element={<JobArchitecturePage />} />
            <Route path="recruitment" element={<RecruitmentPage />} />
                        <Route path="onboarding" element={<OnboardingPage />} />
            <Route path="leave" element={<LeavePage />} />
            <Route path="payroll" element={<PayrollPage />} />
            <Route path="benefits" element={<BenefitsPage />} />
            <Route path="shifts" element={<ShiftsPage />} />
            <Route path="documents" element={<DocumentsPage />} />
          </Route>
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
