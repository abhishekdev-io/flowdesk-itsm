import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Button } from "@/components/ui/button";
import {
  MoreVertical,
  Wrench,
  Undo2,
  RotateCcw,
  Archive,
  Pencil,
} from "lucide-react";
import type { AssetSummaryResponse } from "@/types/asset.types";
import {
  useMoveToMaintenance,
  useReturnFromMaintenance,
  useRestoreAsset,
  useDecommissionAsset,
} from "@/hook/useAssetQueries";

interface AssetRowActionsProps {
  workspaceId: string;
  asset: AssetSummaryResponse;
  onEdit: (id: string) => void;
}

const AssetRowAction = ({
  workspaceId,
  asset,
  onEdit,
}: AssetRowActionsProps) => {
  const moveToMaintenance = useMoveToMaintenance(workspaceId);
  const returnFromMaintenance = useReturnFromMaintenance(workspaceId);
  const restore = useRestoreAsset(workspaceId);
  const decommission = useDecommissionAsset(workspaceId);

  const isPending =
    moveToMaintenance.isPending ||
    returnFromMaintenance.isPending ||
    restore.isPending ||
    decommission.isPending;

  return (
    <DropdownMenu>
      <DropdownMenuTrigger
        render={
          <Button
            variant="ghost"
            size="icon"
            className="h-8 w-8 shrink-0"
            disabled={isPending}
            onClick={(e) => e.stopPropagation()}
            aria-label="Asset actions"
          />
        }
      >
        <MoreVertical className="h-4 w-4" />
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" onClick={(e) => e.stopPropagation()}>
        <DropdownMenuItem onClick={() => onEdit(asset.id)} className="gap-2">
          <Pencil className="h-3.5 w-3.5" />
          Edit
        </DropdownMenuItem>

        {asset.status === "Active" && (
          <>
            <DropdownMenuItem
              onClick={() => moveToMaintenance.mutate(asset.id)}
              className="gap-2"
            >
              <Wrench className="h-3.5 w-3.5" />
              Move to maintenance
            </DropdownMenuItem>
            <DropdownMenuItem
              onClick={() => decommission.mutate(asset.id)}
              className="gap-2 text-destructive focus:text-destructive"
            >
              <Archive className="h-3.5 w-3.5" />
              Decommission
            </DropdownMenuItem>
          </>
        )}

        {asset.status === "UnderMaintenance" && (
          <>
            <DropdownMenuItem
              onClick={() => returnFromMaintenance.mutate(asset.id)}
              className="gap-2"
            >
              <Undo2 className="h-3.5 w-3.5" />
              Return from maintenance
            </DropdownMenuItem>
            <DropdownMenuItem
              onClick={() => decommission.mutate(asset.id)}
              className="gap-2 text-destructive focus:text-destructive"
            >
              <Archive className="h-3.5 w-3.5" />
              Decommission
            </DropdownMenuItem>
          </>
        )}

        {asset.status === "Decommissioned" && (
          <DropdownMenuItem
            onClick={() => restore.mutate(asset.id)}
            className="gap-2"
          >
            <RotateCcw className="h-3.5 w-3.5" />
            Restore asset
          </DropdownMenuItem>
        )}
      </DropdownMenuContent>
    </DropdownMenu>
  );
};

export default AssetRowAction;
