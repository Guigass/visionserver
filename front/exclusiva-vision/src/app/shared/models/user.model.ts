import { Claim } from "./claim.model";

export interface User {
    id: string;
    email: string;
    name: string;
    lastname: string;
    claims: Claim[];
}