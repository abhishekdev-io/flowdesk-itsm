export interface RegisterValues {
  Email: string;
  Username: string;
  Password: string;
  ConfirmPassword: string;
  AvatarUrl?: string;
}

export interface LoginValues {
  Email: string;
  Password: string;
}

export type AuthResponse = {  
  userId: string;
  userName: string;
  email: string;
  avatarUrl: string;
  accessToken: string;
  refreshToken: string;
}