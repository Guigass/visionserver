import { User } from "./user.model";

export interface Login {
    accessToken: string;
    expiresIn: number;
    user: User;
}