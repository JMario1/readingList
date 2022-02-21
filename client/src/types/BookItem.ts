export interface BookItem {
  bookId: string
  createdAt: string
  bookName: string
  author: string
  currentPageNumber: number
  done: boolean
  bookCoverUrl?: string
}
