# UCAA HR MANUAL - UNIMPLEMENTED FEATURES ANALYSIS
**Date: April 7, 2026 | Manual Reference: UCAA/DHRA/MAN/02 (JULY 2022, REV 00)**

---

## EXECUTIVE SUMMARY

The UCAA HR Manual contains comprehensive policies across 277 pages covering HR management, payroll, benefits, and organizational governance. Based on current system implementation, the following features remain unimplemented:

---

## 1. LEAVE MANAGEMENT (Partial)

### ✓ IMPLEMENTED
- Basic leave request structure
- Leave types enumeration

### ✗ NOT IMPLEMENTED
- **Annual Leave Entitlements**: Specific leave balances by job level and years of service
- **Maternity Leave**: Full maternity leave policy and approval workflows
- **Sick Leave**: Medical certification requirements and limits
- **Unpaid Leave**: Processing and approval rules
- **Study Leave**: Educational leave policy with conditions
- **Compassionate Leave**: Bereavement leave policies
- **Leave Carry-over Rules**: Unused leave management and expiry
- **Leave Payouts**: Final payment calculations for unused leave (especially on termination)
- **Leave Scheduling & Conflicts**: Manager approval with conflict detection
- **Leave Balances Tracking**: Real-time balance visibility and forecasting
- **Leave Audit Trails**: Complete leave history with approval chains

---

## 2. PAYROLL & SALARY STRUCTURE (Partial)

### ✓ IMPLEMENTED
- Basic PayrollRecord entity
- Basic salary fields (BasicSalary, GrossPay, NetPay)

### ✗ NOT IMPLEMENTED

#### SALARY SCALES (Critical - Manual Pages 175-177)
- **Inspector Salary Grades** (Multiple levels with notches):
  - INS 1 (Inspector)
  - INS 2 (Senior Inspector) 
  - INS 3 (Principal Inspector)
  - INS 4 (Manager Inspector)
  - Each with 5+ notch progressions
  - Multiple sectors (a, b, c) with different salary bands
  
- **Mission Critical Staff** salary adjustments
- **Sector-specific Salary Scales** for:
  - DAAS (Airports & Aviation Security)
  - DANS (Air Navigation Services)  
  - DSSER (Safety, Security & Economic Regulation)

#### ALLOWANCES (Not Tracked)
- **Duty Allowances**: 7-10% of basic salary by grade
- **Rating Allowances** (DANS): 12-15% for qualified air traffic controllers
- **Professional Allowances** (DANS): 9% upon qualification
- **Operational Allowances**: Various department-specific allowances
- **Housing Allowance**: Listed as separate field
- **Transport Allowance**: Separate allowance (partially present in schema)
- **Meal Allowance**: Benefits module partially addresses
- **Custom Allowances**: Per-employee or department-specific

#### DEDUCTIONS (Not Tracked)
- **PAYE Tax**: Government income tax calculation
- **Pension Deductions**: Mandatory retirement contributions
- **Loan Deductions**: Employee loan repayments
- **Insurance Deductions**: Health insurance, life insurance
- **Union/Professional Fees**: Membership deductions
- **Custom Deductions**: Disciplinary fines, etc.

####SALARY PROCESSING
- **Payroll Cycles**: Frequency determination (Monthly, Bi-weekly)
- **Payroll Run Generation**: Batch processing for multiple employees
- **Payroll Approval Workflows**: Finance approval before payment
- **Payment Method Management**: Bank transfers, cash, etc.
- **Payroll Reports**: Summary reports by department, grade, etc.
- **PAYE Reconciliation**: Tax reporting to government
- **Statutory Reporting**: Income declaration for tax authorities

---

## 3. DISCIPLINARY PROCEDURES (Not Implemented)

### ✗ NOT IMPLEMENTED (Manual Pages 124-127)

#### DISCIPLINARY OFFENCES (Comprehensive List)
Tracked offences include:
- Drunkenness on duty
- Consumption of illegal drugs
- Insubordination and refusal of lawful orders
- Neglect of duty / absence without permission
- Abusive language, assault
- Damage/misuse of company property
- Forgery, falsifying documents
- Theft, fraud, embezzlement
- Unauthorized access/alteration of records
- Striking/illegal strikes
- Endangering safety of others
- Soliciting/accepting bribes
- Non-adherence to policies

#### DISCIPLINARY PROCESS
- Complaint filing and investigation
- Disciplinary hearing procedures
- Committee composition and fairness rules
- Standard of proof (balance of probabilities)
- Employee representation rights
- Majority vote decisions
- Appeal mechanisms
- Sanction types (warnings, suspension, termination)
- Disciplinary record tracking

---

## 4. EMPLOYEE ONBOARDING (Partial)

### ✓ IMPLEMENTED
- Basic onboarding record structure
- Task/item tracking

### ✗ NOT IMPLEMENTED
- **Pre-hire Onboarding**: Job offer acceptance, documentation verification
- **Induction Checklist**: Standardized tasks per department/role
- **System Access Setup**: IT account creation, access provisioning
- **Equipment Allocation**: Laptop, phone, security badges
- **Training Programs**: Mandatory training modules
- **Probation Period Management**: 6-month or custom probation tracking
- **Probation Review**: Confirmation of employment after probation
- **Mentor Assignment**: Pairing new employees with senior staff
- **Welcome Materials**: Employee handbook distribution
- **Policy Acknowledgements**: Signed confirmations of policy understanding

---

## 5. PERFORMANCE MANAGEMENT (Partial)

### ✓ IMPLEMENTED (via Appraisal structure reference)
- Basic appraisal record framework

### ✗ NOT IMPLEMENTED
- **Performance Metrics**: KPI definition and tracking
- **Goal Setting**: SMART objectives definition
- **360-Degree Feedback**: Multi-rater assessment
- **Performance Score Calculation**: Weighted criteria scoring
- **Rating Levels**: Poor, Below Average, Average, Above Average, Excellent
- **Performance Review Cycles**: Quarterly, bi-annual, annual
- **Self-Assessment**: Employee self-rating capability
- **Calibration Meetings**: Manager discussions on ratings
- **Performance Improvement Plans (PIP)**: Structured improvement plans
- **Performance-Based Bonuses**: Link to compensation (if applicable)

---

## 6. EMPLOYEE TRANSFERS & PROMOTIONS (Not Implemented)

### ✗ NOT IMPLEMENTED

#### TRANSFERS
- Transfer request submission
- Transfer approval workflow
- Transfer effective dates
- Salary adjustment on transfer
- Clearance from current department
- Exit documentation

#### PROMOTIONS
- Promotion eligibility criteria (seniority, qualifications)
- Promotion announcement
- Promotion approval authority
- Promotion effective dates
- Salary increment rules
- Acting appointments

#### JOB POSTING  & INTERNAL ADVERT
- Internal job availability posting
- Application windows
- Selection criteria
- Interview scheduling
- Offer management
- Notification of results

---

## 7. EMPLOYEE TERMINATION & SEPARATION (Not Implemented)

### ✗ NOT IMPLEMENTED
- **Resignation Processing**: 
  - Resignation letter acceptance
  - Notice period enforcement (typically 30 days)
  - Resignation approval workflow
  - Effective date management
  
- **Retirement**:
  - Pension calculation
  - Gratuity calculation (based on service years)
  - Final leave payout
  - Retirement benefits summary
  
- **Termination for Cause**:
  - Disciplinary termination process
  - Final settlement calculations
  - Property recovery checklist
  
- **Termination Checklist**:
  - Exit interview
  - Clearance form (finance, equipment, property)
  - Final salary payment + benefits
  - Gratuity/service award
  - PAYE finalization
  - Pension fund withdrawal
  
- **Offboarding**:
  - ID card recovery
  - Equipment return
  - System access revocation
  - Exit date management
  - Final payslip delivery

---

## 8. RECRUITMENT & HIRING (Partial)

### ✓ IMPLEMENTED
- Basic job requisition tracking
- Job application management
- Application status workflow

### ✗ NOT IMPLEMENTED
- **Job Description Management**: Standard job descriptions per position
- **Competency Matching**: Required skills/qualifications
- **Interview Management**: Interview scheduling, notes, scoring
- **Reference Checks**: Contact and document references
- **Background Checks**: Security/police vetting (mentioned for DANS/DAAS)
- **Medical Screening**: Pre-employment medical clearance
- **Offer Letter Generation**: Automated offer creation
- **Offer Negotiation**: Counter-offer tracking  
- **Onboarding Coordination**: Integration with onboarding module
- **Recruitment Metrics**: Hiring KPIs, time-to-hire

---

## 9. CONFLICT OF INTEREST MANAGEMENT (Mentioned but Not Implemented)

### ✗ NOT IMPLEMENTED (Manual Page 164-167)
- **Conflict of Interest Reporting**: Declaration forms
- **Conflict Resolution Matrix**: Decision authority by conflict type
- **Disclosure Requirements**: Annual or per-transaction
- **Recusal Procedures**: How managers handle conflicts
- **Documentation**: Conflict records and resolutions

---

## 10. ATTENDANCE & TIME TRACKING (Partial)

### ✓ IMPLEMENTED
- Basic attendance records
- Check-in/Check-out tracking

### ✗ NOT IMPLEMENTED
- **Shift Scheduling**: Automated shift assignment
- **Overtime Tracking**: Hours beyond standard 8 hours
- **Overtime Compensation**: Overtime payment calculation
- **Authorized Absences**: Approved time off management
- **Unauthorized Absences**: Tracking and notification
- **Tardiness Tracking**: Late arrival patterns
- **Time Sheet Approval**: Manager sign-off on hours
- **Attendance Reports**: Department/individual summaries
- **Biometric Integration**: (If applicable) Clock-in system

---

## 11. TRAINING & DEVELOPMENT (Mentioned but Not Implemented)

### ✗ NOT IMPLEMENTED
- **Training Needs Assessment**: Skills gap identification
- **Training Program Catalog**: Available courses/certifications
- **Training Registration**: Employee enrollment
- **Training Path Definition**: Career development paths
- **Certification Tracking**: Professional certifications maintained
- **Training Records**: Completion dates, scores, certificates
- **Competency Matrix**: Skills vs. roles mapping
- **Succession Planning**: Identified successors for key roles
- **Mentoring Program**: Formal mentoring assignments

---

## 12. REEMPLOYMENT & REHIRING (Not Implemented)

### ✗ NOT IMPLEMENTED (Manual Page 44 referenced)
- **Reemployment Eligibility**: Who can be rehired
- **Rehiring Process**: Simplified hiring for former employees
- **Service Continuity**: Benefits for former staff returning
- **Reference from Previous Service**: Considerations for prior employment

---

## 13. SECONDMENT (Mentioned but Not Defined)

### ✗ NOT IMPLEMENTED (Manual Page 44 referenced)
- **Secondment Request**: Loan to other organizations
- **Secondment Agreement**: Terms and conditions
- **Secondment Duration**: Fixed-term assignment
- **Return to Org**: Post-secondment handover
- **Cost Recovery**: Who bears salary costs

---

## 14. TELEWORKING POLICY (Partial)

### ✓ IMPLEMENTED
- Basic telework policy framework mentioned

### ✗ NOT IMPLEMENTED (Manual Pages 239-240)
- **Telework Application**: Formal application process
- **Eligibility Assessment**: Job suitability evaluation
- **Supervisor Approval**: Management approval workflow
- **Training Requirement**: Mandatory telework orientation
- **Telecommuting Agreement**: Signed targets and expectations
- **Performance Monitoring**: Remote worker oversight
- **Equipment Provision**: Company assets for home use
- **Internet Reimbursement**: (If applicable)

---

## 15. BENEFITS ADMINISTRATION (Partial)

### ✓ IMPLEMENTED
- Benefit plans (Medical, Pension, Life Insurance, Housing, Meal, etc.)
- Employee enrollment
- Contribution tracking

### ✗ NOT IMPLEMENTED
- **Benefits Eligibility**: Who qualifies for which benefits
- **Benefit Usage Tracking**: Claims, deductibles, co-pays
- **Benefit Cost Analysis**: Comparative cost by grade
- **Benefit Changes**: Open enrollment periods
- **NSSF/Pension Integration**: Government/mandatory pensions
- **Health Insurance Claims**: Claims processing
- **Life Insurance Beneficiary**: Named beneficiaries
- **Benefits Statements**: Annual benefits summary per employee

---

## 16. PERSONNEL FILE & RECORDS MANAGEMENT (Not Implemented)

### ✗ NOT IMPLEMENTED
- **Personnel File Structure**: What documents must be filed
- **Document Retention**: Storage and archival rules
- **File Access Control**: Who can view personnel files
- **Confidentiality**: Privacy protection of employee records
- **Amendments**: How errors in files are corrected
- **Document Management System**: (If electronic)
- **Employee Data Privacy**: GDPR/local compliance

---

## 17. CONFIDENTIALITY & SECRECY OATH (Not Implemented)

### ✗ NOT IMPLEMENTED (Manual Page 185)
- **Confidentiality Agreement**: Swearing-in document
- **Secret Information**: Definition and classification
- **Non-Disclosure Requirements**: Legal obligations
- **Post-Employment**: Confidentiality after separation

---

## 18. LEAVE PAYOUT ON TERMINATION (Not Implemented)

### ✗ NOT IMPLEMENTED
- **Annual Leave Payout**: Accrued leave payment at separation
- **Sick Leave Payout**: If applicable by policy
- **Gratuity Calculation**: Service amount per year
- **Severance Pay**: (If applicable)
- **Final Settlement**: Total termination payment calculation
- **Tax Treatment**: PAYE on termination amounts

---

## 19. EMPLOYMENT TYPES & STATUS (Partial)

### ✓ IMPLEMENTED
- Permanent, Contract, Internship, Casual (in schema)

### ✗ NOT IMPLEMENTED
- **Status-Specific Entitlements**: Different benefits per type
- **Contract Renewals**: End-of-contract evaluation
- **Apprenticeship Programs**: (If applicable)
- **Probationary Status**: Separate treatment during probation

---

## 20. SALARY REVIEW & INCREMENT (Not Implemented)

### ✗ NOT IMPLEMENTED
- **Annual Review Cycle**: When salaries are reviewed
- **Increment approval**: Authority and process
- **Increment calculation**: Percentage rules
- **Cost of Living Adjustment (COLA)**: Inflation-based raises
- **Merit-based Increments**: Performance-linked raises
- **Promotion Increments**: Salary jump on promotion

---

## 21. DUTY HOURS & WORKING TIME (Not Implemented)

### ✗ NOT IMPLEMENTED
- **Standard Working Hours**: Official work hours (e.g., 8am-5pm, 8hrs/day)
- **Flex Time Policy**: (If applicable)
- **Core Hours**: Must-be-present times
- **Time Tracking Tool**: System for monitoring hours
- **Workload Management**: Preventing excessive overtime

---

## 22. GRIEVANCE & DISPUTE RESOLUTION (Not Implemented)

### ✗ NOT IMPLEMENTED
- **Grievance Submission**: Formal complaint mechanism
- **Grievance Levels**: Escalation (informal → formal → arbitration)
- **Grievance Timeline**: Response timeframes
- **Mediation Process**: Conflict resolution
- **Appeals**: Right to appeal decisions
- **Arbitration**: External dispute resolution

---

## 23. WORKPLACE CONDUCT & DISCIPLINE (Partial)

### ✓ IMPLEMENTED
- Basic disciplinary procedure reference

### ✗ NOT IMPLEMENTED
- **Code of Conduct**: Detailed conduct rules
- **Dress Code**: Official standards (if applicable)
- **Use of Equipment**: IT, company property rules
- **Social Media Policy**: Professional conduct guidelines
- **Alcohol/Drug Policy**: Zero-tolerance procedures
- **Sexual Harassment**: Complaint and investigation
- **Discrimination**: Protected characteristics
- **Whistleblower Protection**: Reporting retaliation

---

## 24. MONITORING & SURVEILLANCE (Not Implemented)

### ✗ NOT IMPLEMENTED
- **Email Monitoring**: (If applicable)
- **Internet Usage**: Web access logs and policies
- **CCTV**: Video surveillance policies
- **Search & Seizure**: When employer can inspect
- **Right to Privacy**: Balance with employer need

---

## 25. COLLECTIVE AGREEMENTS & UNION RELATIONS (Not Implemented)

### ✗ NOT IMPLEMENTED
- **Union Recognition**: Which unions recognized
- **Collective Bargaining**: Agreement terms
- **Dues Collection**: Union fee deductions
- **Shop Stewards**: Employee representatives
- **Dispute Resolution**: Union-level grievance steps

---

## 26. OCCUPATIONAL HEALTH & SAFETY (Not Implemented)

### ✗ NOT IMPLEMENTED
- **Health & Safety Committee**: Oversight body
- **Incident Reporting**: Accident/injury documentation
- **First Aid**: Trained first responders
- **Safety Training**: Induction safety Modules
- **Hazard Assessment**: Risk identification
- **OSHA Compliance**: (If applicable)
- **Ergonomics**: Workspace setup

---

## 27. EMPLOYEE ASSISTANCE PROGRAM (Not Implemented)

### ✗ NOT IMPLEMENTED
- **Mental Health Support**: Counseling services
- **Wellness Programs**: Health initiatives
- **Substance Abuse**: Treatment programs
- **Family Support**: Work-life balance programs

---

## 28. PERFORMANCE BONUS & INCENTIVES (Not Implemented)

### ✗ NOT IMPLEMENTED
- **Bonus Structure**: Calculation rules
- **Incentive Plans**: Performance-based rewards
- **Commission**: (If applicable)
- **Retention Bonus**: Key employee incentives

---

## 29. RETIREMENT & PENSION (Partial)

### ✓ IMPLEMENTED
- Pension represented in benefits module

### ✗ NOT IMPLEMENTED
- **Pension Plan Details**: NSSF vs. occupational pension
- **Contribution Rates**: Employer/employee split
- **Vesting Schedule**: When benefits become available
- **Retirement Age**: Official retirement date policy
- **Pension Projection**: Estimated retirement income
- **Pension Withdrawal**: Options at retirement

---

## SALARY STRUCTURE DETAILS (Manual Pages 175-177)

The manual provides detailed salary scales that need implementation:

### INSPECTOR GRADES (Mission Critical - DAAS/DSSER FSS)

#### INS 1 - Inspector
- **Band Sector A**: 6,000,000 - 7,714,284 (Notch size: 428,571)
- **Band Sector B**: 8,142,855 - 9,857,139 (Notch size: 428,571)  
- **Band Sector C**: 10,285,710 - 24,642,861 (Notch size: 1,071,429)

#### INS 2 - Senior Inspector
- **Band Sector A**: 10,000,000 - 12,858,144 (Notch size: 714,286)
- **Band Sector B**: 13,571,430 - 16,428,574 (Notch size: 714,286)
- **Band Sector C**: 17,142,858 - 24,642,861

#### INS 3 - Principal Inspector
- **Band Sector A**: 15,000,000 - 19,287,716 (Notch size: 1,071,429)
- **Band Sector B**: 20,357,145 - 24,642,861
- **Band Sector C**: 21,428,574 - 29,571,428

#### INS 4 - Manager Inspector
- **Band Sector A**: 18,000,000 - 23,142,856 (Notch size: 1,285,714)
- **Band Sector B**: 24,428,570 - 29,571,428
- **Band Sector C**: 25,714,284 - 29,571,428

### ALLOWANCES BY DEPARTMENT

#### DAAS (Airports & Aviation Security)
- **Applicable Scale**: Mission Critical Scales (A)
- **Additional Allowances**: None (Salary inclusive)

#### DANS (Air Navigation Services)
- **Applicable Scale**: Mission Critical Scales (A)
- **Additional Allowances**:
  - **Rating Allowances** (Air Traffic Management Officers):
    - 1-2 ratings: 12% of basic salary
    - 3+ ratings: 15% of basic salary
  - **Professional Allowances** (Other departments):
    - 9% of basic salary (upon attaining specific DANS qualifications)

#### DSSER (Non-FSS)
- **Applicable Scale**: Mission Critical Scales (A)
- **Duty Allowances** (% of basic salary):
  - Officer: 7%
  - Senior Officer: 7%
  - Principal Officer: 8%
  - Manager: 10%

#### FSS (Flight Safety Standards) - Inspectors
- **Applicable Scale**: Retention Salary Scales
- **Duty Allowances** (% of basic salary):
  - Inspector: 7%
  - Senior Inspector: 7%
  - Principal Inspector: 8%
  - Manager: 10%

---

## PRIORITY IMPLEMENTATION ROADMAP

### PHASE 1 (Critical Foundation)
1. **Salary Scales & Grades** - Implement all inspector grades and sector bands
2. **Payroll Deductions** - PAYE, pension, insurance handling
3. **Leave Variants** - Maternity, sick, study, compassionate, unpaid
4. **Termination Processing** - Gratuity, final settlement, clearance

### PHASE 2 (Core Operations)
5. **Disciplinary Process** - Complete procedure and offence tracking
6. **Transfer & Promotion** - Job posting, approval workflows
7. **Performance Management** - Goal setting, review cycles, appraisals
8. **Attendance Optimization** - Shift scheduling, overtime tracking

### PHASE 3 (Compliance & Governance)
9. **Grievance Handling** - Multi-level resolution process
10. **Conflict of Interest** - Declaration and resolution
11. **Confidentiality/Secrecy** - NDA management
12. **Training & Development** - Certification, succession planning

### PHASE 4 (Enhancement)
13. **Telework Management** - Application and tracking
14. **Benefits Optimization** - Claims, eligibility rules
15. **Employee Records** - Personnel file management
16. **Health & Safety** - Incident reporting, safety programs

---

## OBSERVATIONS & NOTES

1. **Salary Scales Are Complex**: The multi-sector, multi-notch system requires careful database design to avoid hardcoding
2. **Allowances Are Flexible**: Different departments have vastly different allowance structures - needs parameterization
3. **Legal Requirements**: Disciplinary, grievance, and termination procedures are heavily formal - must track compliance precisely
4. **Pension Integration**: NSSF or occupational pension handling is critical - may require external system integration
5. **Organizational Structure**: Clear department-to-allowance mapping is essential (see Page 269 chart)
6. **Leave Complexity**: Multiple leave types with different rules and payouts create complex business logic
7. **Training & Skills**: The manual emphasizes professional development - suggests a learning management feature would be valuable
8. **Confidentiality**: Secrecy oath and information classification suggest document management is important

---

**Generated on: April 7, 2026**
**System: UCAA HRMS Analysis**
**Total Sections Analyzed: 28 core HR areas**
**Estimated Implementation Effort: Large-scale (6-12 months for complete system)**
