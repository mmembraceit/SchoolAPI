export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  userId: string;
  role: string;
}

export interface AuthUser {
  userId: string;
  role: string;
}
