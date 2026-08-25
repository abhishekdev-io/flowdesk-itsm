import { useParams } from "react-router";
import { useState, useMemo } from "react";
import { useDebounce } from "@/hook/useDebounce";
import Spinner from "@/components/Spinner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import type {
  AssetSummaryResponse,
  AssetFilterParams,
} from "@/types/asset.types";
import { useEntityAccess } from "@/hook/useEntityAccess";
import { useAssets, useAssetStats } from "@/hook/useAssetQueries";
import { useAssetCategories } from "@/hook/useCategoryQueries";
import {
  Search,
  Plus,
  Boxes,
  Cpu,
  Wrench,
  Archive,
  ChevronLeft,
  ChevronRight,
  Layers,
  CircleDot,
  ArrowUpDown,
} from "lucide-react";
import AssetFormDialog from "./components/AssetFormDialog";
import AssetRowActions from "./components/AssetRowAction";

const STATUS_STYLES: Record<
  AssetSummaryResponse["status"],
  { badge: string; dot: string }
> = {
  Active: {
    badge:
      "bg-emerald-500/10 text-emerald-700 dark:text-emerald-400 border-emerald-500/20",
    dot: "bg-emerald-500",
  },
  UnderMaintenance: {
    badge:
      "bg-amber-500/10 text-amber-700 dark:text-amber-400 border-amber-500/20",
    dot: "bg-amber-500",
  },
  Decommissioned: {
    badge: "bg-muted text-muted-foreground border-border",
    dot: "bg-muted-foreground",
  },
};

const SORT_MAP = {
  "name-asc": { sortBy: "Name", descending: false },
  newest: { sortBy: "CreatedAt", descending: true },
  oldest: { sortBy: "CreatedAt", descending: false },
  status: { sortBy: "Status", descending: false },
} as const;

type SortKey = keyof typeof SORT_MAP;

const PAGE_SIZE = 20;

const formatDate = (iso: string) =>
  new Date(iso).toLocaleDateString(undefined, {
    year: "numeric",
    month: "short",
    day: "numeric",
  });

const AssetPage = () => {
  const { workspaceId } = useParams();
  const { canCreate, canUpdate } = useEntityAccess("assets");
  const { data: categories, isLoading: categoriesLoading } = useAssetCategories(
    workspaceId!,
  );
  const { data: statsData, isLoading: statsLoading } = useAssetStats(
    workspaceId!,
  );

  const [page, setPage] = useState(1);
  const [searchInput, setSearchInput] = useState("");
  const debouncedSearch = useDebounce(searchInput, 400);
  const [categoryFilter, setCategoryFilter] = useState<string>("all");
  const [statusFilter, setStatusFilter] = useState<
    AssetSummaryResponse["status"] | "all"
  >("all");
  const [sortBy, setSortBy] = useState<SortKey>("name-asc");
  const [createOpen, setCreateOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);

  const filters: AssetFilterParams = useMemo(
    () => ({
      page,
      pageSize: PAGE_SIZE,
      sortBy: SORT_MAP[sortBy].sortBy,
      descending: SORT_MAP[sortBy].descending,
      search: debouncedSearch || undefined,
      categoryId: categoryFilter === "all" ? undefined : Number(categoryFilter),
      status: statusFilter === "all" ? undefined : statusFilter,
    }),
    [page, sortBy, debouncedSearch, categoryFilter, statusFilter],
  );

  const {
    data,
    isLoading: assetsLoading,
    isFetching,
  } = useAssets(workspaceId!, filters);

  const items = data?.items ?? [];

  const stats = statsData ?? {
    total: data?.totalCount ?? 0,
    active: 0,
    maintenance: 0,
    decommissioned: 0,
  };

  const handleSearchChange = (value: string) => {
    setSearchInput(value);
    setPage(1);
  };
  const handleCategoryChange = (value: string | null) => {
    if (value === null) return;
    setCategoryFilter(value);
    setPage(1);
  };
  const handleStatusChange = (
    value: (AssetSummaryResponse["status"] | "all") | null,
  ) => {
    if (value === null) return;
    setStatusFilter(value);
    setPage(1);
  };
  const handleSortChange = (value: SortKey | null) => {
    if (value === null) return;
    setSortBy(value);
    setPage(1);
  };

  if (assetsLoading || categoriesLoading || statsLoading) {
    return (
      <div className="flex h-[50vh] items-center justify-center">
        <Spinner />
      </div>
    );
  }

  return (
    <div className="p-6 max-w-7xl mx-auto space-y-6">
      {/* Top Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-foreground">
            Assets Inventory
          </h1>
          <p className="text-sm text-muted-foreground mt-0.5">
            Monitor, track hardware configurations, and manage equipment
            lifecycles.
          </p>
        </div>
        {canCreate && (
          <Button
            onClick={() => setCreateOpen(true)}
            className="gap-2 shrink-0 shadow-xs"
          >
            <Plus className="h-4 w-4" />
            New asset
          </Button>
        )}
      </div>

      {/* Metric Stat Cards — now backed by useAssetStats, workspace-wide */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        <div className="p-4 rounded-xl border border-border/70 bg-card text-card-foreground shadow-2xs space-y-1">
          <div className="flex items-center justify-between text-muted-foreground">
            <span className="text-xs font-medium uppercase tracking-wider">
              Total
            </span>
            <Boxes className="h-4 w-4 text-primary" />
          </div>
          <p className="text-2xl font-bold tracking-tight">{stats.total}</p>
        </div>

        <div className="p-4 rounded-xl border border-border/70 bg-card text-card-foreground shadow-2xs space-y-1">
          <div className="flex items-center justify-between text-muted-foreground">
            <span className="text-xs font-medium uppercase tracking-wider">
              Active
            </span>
            <Cpu className="h-4 w-4 text-emerald-500" />
          </div>
          <p className="text-2xl font-bold tracking-tight text-emerald-600 dark:text-emerald-400">
            {stats.active}
          </p>
        </div>

        <div className="p-4 rounded-xl border border-border/70 bg-card text-card-foreground shadow-2xs space-y-1">
          <div className="flex items-center justify-between text-muted-foreground">
            <span className="text-xs font-medium uppercase tracking-wider">
              Maintenance
            </span>
            <Wrench className="h-4 w-4 text-amber-500" />
          </div>
          <p className="text-2xl font-bold tracking-tight text-amber-600 dark:text-amber-400">
            {stats.maintenance}
          </p>
        </div>

        <div className="p-4 rounded-xl border border-border/70 bg-card text-card-foreground shadow-2xs space-y-1">
          <div className="flex items-center justify-between text-muted-foreground">
            <span className="text-xs font-medium uppercase tracking-wider">
              Decommissioned
            </span>
            <Archive className="h-4 w-4 text-muted-foreground" />
          </div>
          <p className="text-2xl font-bold tracking-tight text-muted-foreground">
            {stats.decommissioned}
          </p>
        </div>
      </div>

      {/* Filter & Search Toolbar */}
      <div className="p-3.5 rounded-xl border border-border/70 bg-card/60 backdrop-blur-xs shadow-2xs flex flex-col md:flex-row items-center gap-3 justify-between">
        <div className="relative w-full md:w-96">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Search by asset tag, name, or vendor..."
            value={searchInput}
            onChange={(e) => handleSearchChange(e.target.value)}
            className="pl-9 bg-background/80 text-sm h-9"
          />
        </div>

        <div className="flex flex-wrap items-center gap-2.5 w-full md:w-auto justify-start md:justify-end">
          <Select value={categoryFilter} onValueChange={handleCategoryChange}>
            <SelectTrigger className="w-full sm:w-auto min-w-35 bg-background/50 text-xs">
              <div className="flex items-center gap-1.5 truncate">
                <Layers className="h-3.5 w-3.5 text-muted-foreground shrink-0" />
                <span className="text-muted-foreground font-medium">
                  Category:
                </span>
                <SelectValue placeholder="All">
                  {(value: string) =>
                    value === "all"
                      ? "All"
                      : (categories?.find((c) => String(c.id) === value)
                          ?.name ?? "All")
                  }
                </SelectValue>
              </div>
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All</SelectItem>
              {categories?.map((c) => (
                <SelectItem key={c.id} value={String(c.id)}>
                  {c.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>

          {/* Status Filter */}
          <Select value={statusFilter} onValueChange={handleStatusChange}>
            <SelectTrigger className="w-full sm:w-auto min-w-35 bg-background/50 text-xs">
              <div className="flex items-center gap-1.5 truncate">
                <CircleDot className="h-3.5 w-3.5 text-muted-foreground shrink-0" />
                <span className="text-muted-foreground font-medium">
                  Status:
                </span>
                <SelectValue placeholder="All" />
              </div>
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All</SelectItem>
              <SelectItem value="Active">Active</SelectItem>
              <SelectItem value="UnderMaintenance">Maintenance</SelectItem>
              <SelectItem value="Decommissioned">Decommissioned</SelectItem>
            </SelectContent>
          </Select>
 
          {/* Sort Selector */}
          <Select value={sortBy} onValueChange={handleSortChange}>
            <SelectTrigger className="w-full sm:w-auto min-w-38.75 bg-background/50 text-xs">
              <div className="flex items-center gap-1.5 truncate">
                <ArrowUpDown className="h-3.5 w-3.5 text-muted-foreground shrink-0" />
                <span className="text-muted-foreground font-medium">Sort:</span>
                <SelectValue placeholder="Name (A-Z)" />
              </div>
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="name-asc">Name (A-Z)</SelectItem>
              <SelectItem value="newest">Newest First</SelectItem>
              <SelectItem value="oldest">Oldest First</SelectItem>
              <SelectItem value="status">Status</SelectItem>
            </SelectContent>
          </Select>
        </div>
      </div>

      {/* Asset Items List */}
      <div
        className={`space-y-2.5 transition-opacity ${isFetching ? "opacity-60" : "opacity-100"}`}
      >
        {items.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-16 text-center border border-dashed border-border/80 rounded-xl bg-card/30">
            <div className="flex h-12 w-12 items-center justify-center rounded-full bg-muted/60 mb-3 text-muted-foreground">
              <Boxes className="h-6 w-6" />
            </div>
            <p className="text-base font-medium text-foreground">
              No assets found
            </p>
            <p className="text-xs text-muted-foreground mt-1 max-w-sm">
              {debouncedSearch ||
              categoryFilter !== "all" ||
              statusFilter !== "all"
                ? "No equipment matches your current search filters. Try adjusting your search query."
                : "No assets have been added to this workspace inventory yet."}
            </p>
          </div>
        ) : (
          items.map((a) => {
            const style =
              STATUS_STYLES[a.status] || STATUS_STYLES.Decommissioned;

            return (
              <div
                key={a.id}
                className="group p-4 rounded-xl border border-border/70 bg-card text-card-foreground hover:bg-accent/20 transition-all flex flex-col md:flex-row md:items-center justify-between gap-4 cursor-pointer"
                // onClick={() => navigate(`/workspaces/${workspaceId}/assets/${a.id}`)} // wire once detail page exists
              >
                <div className="space-y-1.5 min-w-0">
                  <div className="flex items-center gap-2 flex-wrap">
                    <span className="font-mono text-xs font-semibold px-2 py-0.5 rounded-md bg-primary/10 text-primary border border-primary/20">
                      {a.assetTag}
                    </span>
                    <h2 className="text-sm font-semibold tracking-tight text-foreground truncate">
                      {a.name}
                    </h2>
                  </div>

                  <div className="flex items-center gap-2 text-xs text-muted-foreground flex-wrap">
                    <span className="font-medium text-foreground/80">
                      {a.categoryName}
                    </span>
                    {a.manufacturer && (
                      <>
                        <span>•</span>
                        <span>{a.manufacturer}</span>
                      </>
                    )}
                    <span>•</span>
                    <span>added {formatDate(a.createdAt)}</span>
                    {a.status === "Decommissioned" && a.decommissionedAt && (
                      <>
                        <span>•</span>
                        <span className="text-muted-foreground/80">
                          decommissioned {formatDate(a.decommissionedAt)}
                        </span>
                      </>
                    )}
                  </div>
                </div>

                <div className="flex items-center gap-2 shrink-0 self-start md:self-center">
                  <span
                    className={`inline-flex items-center gap-1.5 text-xs px-2.5 py-1 rounded-md font-medium border transition-colors ${style.badge}`}
                  >
                    <span className={`h-1.5 w-1.5 rounded-full ${style.dot}`} />
                    {a.status === "UnderMaintenance" ? "Maintenance" : a.status}
                  </span>
                  {canUpdate && (
                    <AssetRowActions
                      workspaceId={workspaceId!}
                      asset={a}
                      onEdit={setEditingId}
                    />
                  )}
                </div>
              </div>
            );
          })
        )}
      </div>

      {/* Pagination */}
      {data && data.totalCount > 0 && (
        <div className="flex items-center justify-between pt-2">
          <p className="text-xs text-muted-foreground">
            Page {data.page} of {data.totalPages} • {data.totalCount} assets
          </p>
          <div className="flex items-center gap-2">
            <Button
              variant="outline"
              size="sm"
              disabled={!data.hasPreviousPage}
              onClick={() => setPage((p) => p - 1)}
              className="gap-1"
            >
              <ChevronLeft className="h-4 w-4" />
              Prev
            </Button>
            <Button
              variant="outline"
              size="sm"
              disabled={!data.hasNextPage}
              onClick={() => setPage((p) => p + 1)}
              className="gap-1"
            >
              Next
              <ChevronRight className="h-4 w-4" />
            </Button>
          </div>
        </div>
      )}

      {canCreate && (
        <AssetFormDialog
          mode="create"
          workspaceId={workspaceId!}
          open={createOpen}
          onOpenChange={setCreateOpen}
        />
      )}

      {canUpdate && (
        <AssetFormDialog
          mode="edit"
          workspaceId={workspaceId!}
          assetId={editingId ?? undefined}
          open={editingId !== null}
          onOpenChange={(open) => !open && setEditingId(null)}
        />
      )}
    </div>
  );
};

export default AssetPage;
