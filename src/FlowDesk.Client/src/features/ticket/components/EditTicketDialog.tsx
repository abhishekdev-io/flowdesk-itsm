import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { useUpdateTicket } from "@/hook/useTicketQueries";
import type { TicketDetailResponse } from "@/types/ticket.types";
import { zodResolver } from "@hookform/resolvers/zod";
import { Pencil } from "lucide-react";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";

const editTicketSchema = z.object({
  title: z.string().min(3, "Title must be at least 3 characters").max(200),
  description: z.string().min(10, "Description must be at least 10 characters"),
});

type EditTicketFormValues = z.infer<typeof editTicketSchema>;

interface EditTicketDialogProps {
  workspaceId: string;
  ticket: TicketDetailResponse;
}

const EditTicketDialog = ({ workspaceId, ticket }: EditTicketDialogProps) => {
  const [open, setOpen] = useState(false);
  const updateTicket = useUpdateTicket(workspaceId, ticket.id);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isDirty },
  } = useForm<EditTicketFormValues>({
    resolver: zodResolver(editTicketSchema),
    defaultValues: {
      title: ticket.title,
      description: ticket.description,
    },
  });

  useEffect(() => {
    if (open) {
      reset({ title: ticket.title, description: ticket.description });
    }
  }, [open, ticket.title, ticket.description, reset]);

  const onSubmit = (values: EditTicketFormValues) => {
    updateTicket.mutate(values, {
      onSuccess: () => setOpen(false),
    });
  };
  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger
        render={
          <Button variant="ghost" size="icon" className="h-8 w-8 shrink-0" />
        }
      >
        <Pencil className="h-4 w-4" />
        <span className="sr-only">Edit ticket</span>
      </DialogTrigger>
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>Edit ticket</DialogTitle>
        </DialogHeader>

        <form
          id="edit-ticket-form"
          onSubmit={handleSubmit(onSubmit)}
          className="space-y-4"
        >
          <div className="space-y-1.5">
            <Label htmlFor="title">Title</Label>
            <Input id="title" {...register("title")} />
            {errors.title && (
              <p className="text-xs text-destructive">{errors.title.message}</p>
            )}
          </div>

          <div className="space-y-1.5">
            <Label htmlFor="description">Description</Label>
            <Textarea id="description" rows={5} {...register("description")} />
            {errors.description && (
              <p className="text-xs text-destructive">
                {errors.description.message}
              </p>
            )}
          </div>
        </form>

        <DialogFooter>
          <Button variant="outline" onClick={() => setOpen(false)}>
            Cancel
          </Button>
          <Button
            type="submit"
            form="edit-ticket-form"
            disabled={!isDirty || updateTicket.isPending}
          >
            {updateTicket.isPending ? "Saving…" : "Save changes"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

export default EditTicketDialog;
