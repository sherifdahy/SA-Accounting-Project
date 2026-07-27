# Project Overview

SA Accounting is a desktop-backed accounting office management system for accounting and law offices.

The main goal is to help an office manage:

- client/company records
- company owners
- company tax and address data
- platform accounts for each company
- employee access to companies
- daily employee expense claims
- custody/fund movements
- attachments related to company work
- historical lookup of documents and actions through the domain data

## Target Users

The system is aimed at accounting offices and law offices.

Typical users include:

- office owner or manager
- accountants
- lawyers or legal assistants
- employees who visit government offices or client sites

## Main Business Flow

An employee may go out during the day to handle work for one or more companies.

For each company, the employee records what happened:

- what was paid
- what was received or extracted
- notes about the work
- attachments such as scanned documents, images, receipts, extracts, or PDFs

The employee submits an `ExpenseClaim`.

The office reviews it.

Approved expenses can be settled against the employee custody.

## Platform Automation Goal

Each company can have accounts on different platforms.

An account stores:

- email/login
- password
- target platform
- owning company

Platforms can store selectors used by web scraping or automation.

The long-term goal is to let the system open company-related online platforms internally without exposing raw credentials to employees.

## Important Product Principle

The system should preserve company history naturally through the data model.

For now, there is no generic `ActivityLog` entity. Historical lookup should be possible from real business entities such as:

- `Company`
- `Attachment`
- `ExpenseClaimItem`
- `ExpenseClaim`
- `ExpenseClaimHistory`
- `Movement`
- `Account`
