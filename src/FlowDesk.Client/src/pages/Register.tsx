import { useState } from "react";
import { NavLink, useNavigate } from "react-router";
import { useForm, useWatch } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import {
  Ticket,
  Eye,
  EyeOff,
  Loader2,
  AlertCircle,
  Check,
  X,
} from "lucide-react";

import { Button } from "../components/ui/button";
import { Input } from "../components/ui/input";
import { Label } from "../components/ui/label";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter,
} from "../components/ui/card";
import { Alert, AlertDescription } from "../components/ui/alert";
import { useAuth } from "@/stores/useAuth";

const registerSchema = z
  .object({
    Email: z
      .string()
      .min(1, "Email is required")
      .email("Enter a valid email address"),
    Username: z
      .string()
      .min(5, "At least 5 characters")
      .max(30, "30 characters or fewer")
      .regex(/^[a-zA-Z0-9_.]+$/, "Letters, numbers, . and _ only"),
    Password: z
      .string()
      .min(10, "At least 10 characters")
      .regex(/[a-z]/, "Needs a lowercase letter")
      .regex(/[A-Z]/, "Needs an uppercase letter")
      .regex(/[0-9]/, "Needs a number"),
    ConfirmPassword: z.string().min(1, "Confirm your password"),
  })
  .refine((data) => data.Password === data.ConfirmPassword, {
    message: "Passwords don't match",
    path: ["confirmPassword"],
  });

type RegisterValues = z.infer<typeof registerSchema>;

const passwordChecks = [
  { label: "10+ characters", test: (v: string) => v.length >= 10 },
  { label: "Uppercase letter", test: (v: string) => /[A-Z]/.test(v) },
  { label: "Lowercase letter", test: (v: string) => /[a-z]/.test(v) },
  { label: "Number", test: (v: string) => /[0-9]/.test(v) },
];

const Register = () => {
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    control,
    formState: { errors, isSubmitting },
  } = useForm<RegisterValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      Email: "",
      Username: "",
      Password: "",
      ConfirmPassword: "",
    },
  });

  const signup = useAuth((state) => state.signup);

  const navigate = useNavigate();

  const password = useWatch({ control, name: "Password" }) ?? "";

  const onSubmit = async (values: RegisterValues) => {
    setServerError(null);
    try {
      const payload = {
        ...values,
      };

      await signup(payload);
      navigate("/hub");
    } catch (err) {
      setServerError(
        err instanceof Error
          ? err.message
          : "Couldn't create your account. Please try again.",
      );
    }
  };

  return (
    <div className="min-h-[calc(100vh-4rem)] grid lg:grid-cols-2 bg-(--bg) text-(--text)">
      {/* Brand panel */}
      <div className="hidden lg:flex flex-col justify-center gap-10 border-r border-border bg-(--surface) px-16">
        <div className="flex items-center gap-2">
          <span className="h-8 w-8 rounded-md bg-(--accent-soft) flex items-center justify-center">
            <Ticket className="h-4 w-4 text-accent" />
          </span>
          <span className="font-display text-xl tracking-tight">FlowDesk</span>
        </div>

        <div>
          <h2 className="font-display text-3xl leading-tight tracking-tight mb-3 max-w-sm">
            Set up your workspace in under a minute.
          </h2>
          <p className="text-(--text-muted) max-w-sm leading-relaxed">
            Create an account, open a workspace, and file your first ticket.
          </p>
        </div>
      </div>

      {/* Form panel */}
      <div className="flex items-center justify-center px-6 py-16">
        <Card className="w-full max-w-sm bg-(--surface) border-border">
          <CardHeader>
            <CardTitle className="font-display text-2xl tracking-tight">
              Create your account
            </CardTitle>
            <CardDescription className="text-(--text-muted)">
              Get started with FlowDesk.
            </CardDescription>
          </CardHeader>

          <form onSubmit={handleSubmit(onSubmit)} noValidate>
            <CardContent className="space-y-4">
              {serverError && (
                <Alert variant="destructive" className="py-2.5">
                  <AlertCircle className="h-4 w-4" />
                  <AlertDescription>{serverError}</AlertDescription>
                </Alert>
              )}

              <div className="space-y-1.5">
                <Label htmlFor="email">Email</Label>
                <Input
                  id="email"
                  type="email"
                  autoComplete="email"
                  placeholder="you@company.com"
                  aria-invalid={!!errors.Email}
                  className="bg-(--surface-2) border-border"
                  {...register("Email")}
                />
                {errors.Email && (
                  <p className="text-xs text-(--critical)">
                    {errors.Email.message}
                  </p>
                )}
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="username">Username</Label>
                <Input
                  id="username"
                  type="text"
                  autoComplete="username"
                  placeholder="jane.doe"
                  aria-invalid={!!errors.Username}
                  className="bg-(--surface-2) border-border"
                  {...register("Username")}
                />
                {errors.Username && (
                  <p className="text-xs text-(--critical)">
                    {errors.Username.message}
                  </p>
                )}
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="password">Password</Label>
                <div className="relative">
                  <Input
                    id="password"
                    type={showPassword ? "text" : "password"}
                    autoComplete="new-password"
                    placeholder="••••••••"
                    aria-invalid={!!errors.Password}
                    className="bg-(--surface-2) border-border pr-10"
                    {...register("Password")}
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword((v) => !v)}
                    className="absolute right-2.5 top-1/2 -translate-y-1/2 text-(--text-muted) hover:text-(--text) cursor-pointer"
                    aria-label={
                      showPassword ? "Hide password" : "Show password"
                    }
                    tabIndex={-1}
                  >
                    {showPassword ? (
                      <EyeOff className="h-4 w-4" />
                    ) : (
                      <Eye className="h-4 w-4" />
                    )}
                  </button>
                </div>
                {password.length > 0 && (
                  <ul className="grid grid-cols-2 gap-x-3 gap-y-1 pt-1">
                    {passwordChecks.map((check) => {
                      const passed = check.test(password);
                      return (
                        <li
                          key={check.label}
                          className={`flex items-center gap-1.5 text-xs ${
                            passed ? "text-(--ok)" : "text-(--text-muted)"
                          }`}
                        >
                          {passed ? (
                            <Check className="h-3 w-3 shrink-0" />
                          ) : (
                            <X className="h-3 w-3 shrink-0" />
                          )}
                          {check.label}
                        </li>
                      );
                    })}
                  </ul>
                )}
                {errors.Password && (
                  <p className="text-xs text-(--critical)">
                    {errors.Password.message}
                  </p>
                )}
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="confirmPassword">Confirm password</Label>
                <div className="relative">
                  <Input
                    id="confirmPassword"
                    type={showConfirm ? "text" : "password"}
                    autoComplete="new-password"
                    placeholder="••••••••"
                    aria-invalid={!!errors.ConfirmPassword}
                    className="bg-(--surface-2) border-border pr-10"
                    {...register("ConfirmPassword")}
                  />
                  <button
                    type="button"
                    onClick={() => setShowConfirm((v) => !v)}
                    className="absolute right-2.5 top-1/2 -translate-y-1/2 text-(--text-muted) hover:text-(--text) cursor-pointer"
                    aria-label={showConfirm ? "Hide password" : "Show password"}
                    tabIndex={-1}
                  >
                    {showConfirm ? (
                      <EyeOff className="h-4 w-4" />
                    ) : (
                      <Eye className="h-4 w-4" />
                    )}
                  </button>
                </div>
                {errors.ConfirmPassword && (
                  <p className="text-xs text-(--critical)">
                    {errors.ConfirmPassword.message}
                  </p>
                )}
              </div>
            </CardContent>

            <CardFooter className="flex flex-col gap-4">
              <Button
                type="submit"
                disabled={isSubmitting}
                className="w-full cursor-pointer bg-accent text-(--accent-contrast) font-semibold hover:brightness-110"
              >
                {isSubmitting ? (
                  <>
                    <Loader2 className="h-4 w-4 animate-spin" /> Creating
                    account…
                  </>
                ) : (
                  "Create account"
                )}
              </Button>
              <p className="text-sm text-(--text-muted)">
                Already have an account?{" "}
                <NavLink to="/login" className="text-accent hover:underline">
                  Sign in
                </NavLink>
              </p>
            </CardFooter>
          </form>
        </Card>
      </div>
    </div>
  );
};

export default Register;
