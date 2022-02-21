import { apiEndpoint } from '../config'
import { BookItem } from '../types/BookItem';
import { CreateBookRequest } from '../types/CreateBookRequest';
import Axios from 'axios'
import { UpdateBookRequest } from '../types/UpdateBookRequest';

export async function getBooks(idToken: string): Promise<BookItem[]> {
  console.log('Fetching books')

  const response = await Axios.get(`${apiEndpoint}/readingList`, {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${idToken}`
    },
  })
  console.log('Todos:', response.data)
  return response.data.items
}

export async function createBook(
  idToken: string,
  newTodo: CreateBookRequest
): Promise<BookItem> {
  const response = await Axios.post(`${apiEndpoint}/readingList`,  JSON.stringify(newTodo), {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${idToken}`
    }
  })
  return response.data.item
}

export async function patchBook(
  idToken: string,
  bookId: string,
  updatedTodo: UpdateBookRequest
): Promise<void> {
  await Axios.patch(`${apiEndpoint}/readingList/${bookId}`, JSON.stringify(updatedTodo), {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${idToken}`
    }
  })
}

export async function deleteBook(
  idToken: string,
  bookId: string
): Promise<void> {
  await Axios.delete(`${apiEndpoint}/readingList/${bookId}`, {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${idToken}`
    }
  })
}

export async function getUploadUrl(
  idToken: string,
  bookId: string
): Promise<string> {
  const response = await Axios.post(`${apiEndpoint}/readingList/${bookId}/image`, '', {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${idToken}`
    }
  })
  return response.data.uploadUrl
}

export async function uploadFile(uploadUrl: string, file: Buffer): Promise<void> {
  await Axios.put(uploadUrl, file, {
    headers: {
      'Content-Type': ''
    }
  })
}
