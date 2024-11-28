namespace RecipeShare.Web.ViewModels.PaginationViewModels
{
    public class PaginatedList<T> : List<T>
    {
        public IEnumerable<T> Items { get; private set; }
		public int TotalItems { get; private set; }
		public int PageSize { get; private set; }
		public int CurrentPage { get; private set; }
		public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

		public PaginatedList(IEnumerable<T> items, int totalItems, int currentPage, int pageSize)
		{
			Items = items;
			TotalItems = totalItems;
			CurrentPage = currentPage;
			PageSize = pageSize;
		}

		public bool HasPreviousPage => CurrentPage > 1;
		public bool HasNextPage => CurrentPage < TotalPages;
	}
}
