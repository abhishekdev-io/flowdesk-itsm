import { useState } from "react";
import { NavLink, useLocation, useNavigate } from "react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Ticket, Eye, EyeOff, Loader2, AlertCircle } from "lucide-react";

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
import {useAuth} from "@/stores/useAuth";

const loginSchema = z.object({
  Email: z
    .string()
    .min(1, "Email is required")
    .email("Enter a valid email address"),
  Password: z.string().min(1, "Password is required"),
  remember: z.boolean().optional(),
});

type LoginValues = z.infer<typeof loginSchema>;

const queuePreview = [
  { id: "FD-1042", status: "Escalated", tone: "text-(--critical)" },
  { id: "FD-1038", status: "Assigned", tone: "text-(--text-muted)" },
  { id: "FD-1035", status: "In progress", tone: "text-(--warn)" },
];

const Login = () => {
  const [showPassword, setShowPassword] = useState(false);
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { Email: "", Password: "", remember: false },
  });

  const signin = useAuth((state) => state.signin);
  
  const location = useLocation();
  const from = location.state?.from?.pathname || "/hub";

  const navigate = useNavigate();

  const onSubmit = async (values: LoginValues) => {
    setServerError(null);
    try {
      await signin(values);

      navigate(from, { replace: true });
    } catch (err) {
      setServerError(
        err instanceof Error
          ? err.message
          : "Couldn't sign you in. Check your email and password.",
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
            Every ticket, tracked from first report to final close.
          </h2>
          <p className="text-(--text-muted) max-w-sm leading-relaxed">
            Sign in to pick up where your workspace left off.
          </p>
        </div>

        <div className="rounded-xl border border-border overflow-hidden max-w-sm">
          <div className="px-4 py-2.5 border-b border-border bg-(--surface-2) font-mono text-[10px] uppercase tracking-widest text-(--text-muted)">
            Active queue
          </div>
          {queuePreview.map((t) => (
            <div
              key={t.id}
              className="flex items-center justify-between px-4 py-2.5 border-b border-border last:border-0 text-xs"
            >
              <span className="font-mono text-accent">{t.id}</span>
              <span className={`font-mono ${t.tone}`}>{t.status}</span>
            </div>
          ))}
        </div>
      </div>

      {/* Form panel */}
      <div className="flex items-center justify-center px-6 py-16">
        <Card className="w-full max-w-sm bg-(--surface) border-border">
          <CardHeader>
            <CardTitle className="font-display text-2xl tracking-tight">
              Welcome back
            </CardTitle>
            <CardDescription className="text-(--text-muted)">
              Sign in to your FlowDesk workspace.
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
                <Label htmlFor="password">Password</Label>
                <div className="relative">
                  <Input
                    id="password"
                    type={showPassword ? "text" : "password"}
                    autoComplete="current-password"
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
                {errors.Password && (
                  <p className="text-xs text-(--critical)">
                    {errors.Password.message}
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
                    <Loader2 className="h-4 w-4 animate-spin" /> Signing in…
                  </>
                ) : (
                  "Sign in"
                )}
              </Button>
              <p className="text-sm text-(--text-muted)">
                Don't have an account?{" "}
                <NavLink to="/register" className="text-accent hover:underline">
                  Register
                </NavLink>
              </p>
            </CardFooter>
          </form>
        </Card>
      </div>
    </div>
  );
};

export default Login;
