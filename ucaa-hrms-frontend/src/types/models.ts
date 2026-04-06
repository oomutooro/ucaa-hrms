export interface AuthResponse {
  accessToken: string;
  email: string;
  role: string;
}

export interface DashboardMetrics {
  totalEmployees: number;
  employeesOnLeave: number;
  upcomingShifts: number;
}

export interface Department {
  id: string;
  name: string;
  parentDepartmentId?: string | null;
}

export interface Employee {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  employeeId: string;
  departmentId: string;
  departmentName: string;
  jobTitle: string;
  employmentType: number;
  annualLeaveBalanceDays: number;
}

export interface LeaveRequest {
  id: string;
  employeeId: string;
  employeeName: string;
  leaveType: number;
  status: number;
  startDate: string;
  endDate: string;
  reason: string;
  reviewerComment?: string;
}

export interface PayrollRecord {
  id: string;
  employeeId: string;
  employeeName: string;
  basicSalary: number;
  allowances: number;
  deductions: number;
  netPay: number;
  payPeriod: string;
  notes?: string;
}

export interface ShiftAssignment {
  id: string;
  shiftDate: string;
  shiftType: number;
  employeeId?: string;
  employeeName?: string;
  shiftCode: string;
}

export interface DocumentItem {
  id: string;
  employeeId?: string;
  documentType: number;
  fileName: string;
  contentType: string;
  fileSizeBytes: number;
}
