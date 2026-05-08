export interface Student {
  id: string;
  name: string;
  dateOfBirth: string; // ISO 8601 — e.g. "1999-04-15"
  tenantId: string;
}

export interface CreateStudentRequest {
  name: string;
  dateOfBirth: string;
}

export interface UpdateStudentRequest {
  name: string;
  dateOfBirth: string;
}
