# UCAA HRMS Implementation Roadmap & Milestone Todo

This roadmap turns the HR manual gaps into an execution plan with milestones we can tick off.

## Can This Be Implemented?

Yes. The current system already has a strong base:
- Core employee management
- Job architecture basics (departments, positions, grades)
- Leave baseline
- Payroll baseline
- Onboarding baseline
- Benefits baseline
- Employee profile workflows

The remaining work is substantial but fully feasible using phased delivery.

## Delivery Principles

1. Deliver usable slices per milestone, not giant all-at-once releases.
2. Keep legal/compliance-heavy modules auditable (history, approvals, evidence).
3. Prefer configurable policy tables over hardcoding (salary scales, leave rules, allowances).
4. Ship each milestone with data migration + role-based permissions + tests.
5. Close each milestone with demo scenarios and acceptance checklist.

## Milestone Roadmap

## Milestone 1: Compensation Engine Foundation (Salary + Allowances)

Goal: Implement salary structure from manual (scales, notches, sectors, role-based allowances).

Scope:
- Salary scales model: grade, sector, notch, min/max ranges
- Position-to-scale mapping
- Allowance rules engine (percentage/fixed)
- Mission-critical pay structures (DAAS/DANS/DSSER/FSS)
- Rating/professional/duty allowance configuration
- Salary history and effective dates

Acceptance:
- Position assignment resolves base pay + allowance package automatically
- Historical salary changes are traceable and auditable
- HR can configure scales without code changes

Milestone checklist:
- [ ] Database schema for scales/notches/sectors
- [ ] Salary rule calculation service
- [ ] Allowance rule configuration UI/API
- [ ] Position compensation mapping
- [ ] Effective-date salary history support
- [ ] Unit and integration tests
- [ ] Seed data for known inspector scales
- [ ] Migration scripts verified

## Milestone 2: Payroll Rules & Statutory Deductions

Goal: Upgrade payroll into a full computation and processing module.

Scope:
- Payroll run periods and payroll batches
- PAYE, pension, loan, insurance, custom deductions
- Earnings composition: base, allowances, overtime, adjustments
- Validation and approval workflow
- Payslip generation and export
- Payroll audit logs and rerun safeguards

Acceptance:
- Payroll batch can be generated, reviewed, approved, and posted
- Net pay is reproducible from transparent formulas
- Payslips and statutory breakdowns are available per employee

Milestone checklist:
- [ ] Payroll batch model + status workflow
- [ ] Deduction engine (PAYE/pension/loan/custom)
- [ ] Earnings engine (salary + allowances + overrides)
- [ ] Approval flow (HR -> Finance)
- [ ] Payslip generation
- [ ] Payroll reports and export endpoints
- [ ] Regression tests for payroll formulas

## Milestone 3: Leave Policy Expansion & Leave Accounting

Goal: Implement complete leave policy behavior from manual.

Scope:
- Leave types: annual, maternity, sick, unpaid, study, compassionate
- Policy rules per type: eligibility, evidence, max days, carryover
- Leave balances and accrual logic
- Approval chains and conflict checks
- Leave payout computation for separation scenarios

Acceptance:
- All leave types are requestable with policy-driven validation
- Balance, accrual, carryover, and approvals are accurate and auditable

Milestone checklist:
- [ ] Leave policy configuration tables
- [ ] Expanded leave request workflows
- [ ] Balance/accrual/carryover service
- [ ] Approval matrix and delegation support
- [ ] Leave payout calculator
- [ ] Tests for all leave variants

## Milestone 4: Separation, Exit & Final Settlement

Goal: Cover resignation, termination, retirement, offboarding, and clearance.

Scope:
- Separation case types and lifecycle
- Notice-period handling
- Final settlement calculator (leave payout, gratuity, deductions)
- Clearance checklist (assets, finance, system access)
- Exit interview records and completion tracking

Acceptance:
- Any separation can be processed end-to-end without manual spreadsheets
- Final settlement is explainable and exportable

Milestone checklist:
- [ ] Separation case entity and statuses
- [ ] Final settlement calculation service
- [ ] Clearance workflow and approvals
- [ ] Exit interview and evidence capture
- [ ] Settlement statement output
- [ ] End-to-end tests

## Milestone 5: Performance, Promotion, Transfer, Internal Recruitment

Goal: Connect performance outcomes to movement and growth workflows.

Scope:
- Performance goals, cycles, scoring, moderation
- Appraisal templates and weighted criteria
- Promotion/transfer requests and approvals
- Internal advertisement and candidate shortlisting
- Decision records and effective-date changes

Acceptance:
- Performance reviews are completed in-system with scoring evidence
- Promotion/transfer/internal recruitment flows are operational

Milestone checklist:
- [ ] Performance cycle and template models
- [ ] Goal-setting and appraisal forms
- [ ] Promotion/transfer workflow
- [ ] Internal advert and candidate workflow
- [ ] Decision audit trail
- [ ] Test coverage

## Milestone 6: Discipline, Grievance & Case Management

Goal: Implement formal employee-relations case handling.

Scope:
- Misconduct catalog and disciplinary cases
- Hearing workflow, decisions, sanctions, appeals
- Grievance filing, escalation levels, and resolution tracking
- Case confidentiality controls and evidence handling

Acceptance:
- Disciplinary and grievance processes are policy-compliant and auditable
- Appeals can be tracked to closure

Milestone checklist:
- [ ] Case management entities and states
- [ ] Disciplinary hearing workflow
- [ ] Sanctions and appeal handling
- [ ] Grievance workflow and SLA tracking
- [ ] Secure evidence/document storage links
- [ ] Tests + role/permission checks

## Milestone 7: Training, Competency, Telework & Compliance

Goal: Complete capability and policy compliance modules.

Scope:
- Training catalog, enrollment, completion records
- Certification/competency tracking
- Telework eligibility, agreements, target tracking
- Conflict-of-interest declarations
- Confidentiality/secrecy acknowledgement records

Acceptance:
- HR can track qualifications and policy acknowledgements centrally
- Telework and conflict declarations have full approval trails

Milestone checklist:
- [ ] Training and certification module
- [ ] Competency matrix support
- [ ] Telework request/agreement workflow
- [ ] Conflict-of-interest declarations
- [ ] Confidentiality acknowledgement workflow
- [ ] Compliance reporting endpoints

## Milestone 8: Stabilization, Reporting, Hardening & UAT

Goal: Make the platform production-ready for organization-wide use.

Scope:
- Cross-module reporting dashboards
- Data quality checks and reconciliation tools
- Permission hardening and audit observability
- Performance optimization and migration cleanup
- User acceptance testing and rollout documentation

Acceptance:
- Stakeholders sign off that key HR processes run fully in-system
- Milestone KPIs and UAT scenarios pass

Milestone checklist:
- [ ] Operational and executive reports
- [ ] Data validation and reconciliation utilities
- [ ] Security/permission review completed
- [ ] Load/performance checks completed
- [ ] UAT sign-off pack
- [ ] Go-live checklist

## Working Todo Board (Tick As We Go)

## Current status
- [x] Baseline HRMS platform with employee, leave, onboarding, payroll, benefits foundations
- [ ] Milestone 1 complete
- [ ] Milestone 2 complete
- [ ] Milestone 3 complete
- [ ] Milestone 4 complete
- [ ] Milestone 5 complete
- [ ] Milestone 6 complete
- [ ] Milestone 7 complete
- [ ] Milestone 8 complete

## Next immediate sprint (recommended start)
- [ ] Confirm salary scale source tables from manual pages 175-177
- [ ] Approve canonical grade/sector/notch data model
- [ ] Implement compensation schema migration
- [ ] Build salary + allowance calculation service
- [ ] Expose compensation preview endpoint for employee profile
- [ ] Add admin UI for salary scale maintenance
- [ ] Seed known inspector scales and department allowance policies
- [ ] Run validation scenarios against manual examples

## Tracking conventions

Use these status markers when we update this file:
- [ ] Not started
- [x] Completed

Progress update rule:
- At milestone close, tick the milestone and all acceptance checklist items.
- If scope changes, add a dated note under the milestone.

## Change Log

- 2026-04-07: Initial roadmap and tickable implementation todo created.
