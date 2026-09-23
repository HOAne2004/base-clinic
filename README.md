# BaseClinic

BaseClinic is a web-based clinic appointment management system built with
ASP.NET Core. The project focuses on managing the appointment lifecycle
between patients, doctors, and clinic staff.

## Overview

The initial version focuses on patient authentication, appointment booking,
and appointment check-in. The system is being developed with a layered
architecture to separate domain logic, business logic, data access, and
API presentation.

## Features

### Authentication

- [x] User registration
- [x] User login

### Appointment

- [x] Book appointment
- [x] Appointment check-in

### In Progress

- [ ] Role-Based Access Control
- [ ] Doctor schedule management
- [ ] Queue management
- [ ] Consultation
- [ ] Prescription management
- [ ] Patient medical history

## Architecture

```text
BaseClinic
├── backend
│   ├── BaseClinic.sln
│   ├── BaseClinic.Api
│   ├── BaseClinic.Business
│   ├── BaseClinic.DataAccess
│   └── BaseClinic.Domain
│
└── frontend
    └── Next.js
