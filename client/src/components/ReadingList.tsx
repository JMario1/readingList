import dateFormat from 'dateformat'
import { History } from 'history'
import update from 'immutability-helper'
import * as React from 'react'
import {
  Button,
  Checkbox,
  Divider,
  Grid,
  Header,
  Icon,
  Input,
  Image,
  Loader,
} from 'semantic-ui-react'

import { createBook, deleteBook, getBooks, patchBook } from '../api/readingList-api'
import Auth from '../auth/Auth'
import { BookItem } from '../types/BookItem'

interface BooksProps {
  auth: Auth
  history: History
}

interface BooksState {
  books: BookItem[]
  newBookName: string
  newBookAuthor: string
  loadingBooks: boolean
}

export class ReadingList extends React.PureComponent<BooksProps, BooksState> {
  state: BooksState = {
    books: [],
    newBookName: '',
    newBookAuthor: '',
    loadingBooks: true
  }

  handleNameChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    this.setState({ newBookName: event.target.value })
  }

  handleAuthorChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    this.setState({ newBookAuthor: event.target.value })
  }

  onEditButtonClick = (bookId: string) => {
    this.props.history.push(`/readinglist/${bookId}/edit`)
  }

  onBookCreate = async (event: React.MouseEvent<HTMLButtonElement>) => {
    try {
      const currentPageNumber = 0
      const newBook = await createBook(this.props.auth.getIdToken(), {
        bookName: this.state.newBookName,
        author: this.state.newBookAuthor,
        currentPageNumber
      })
      console.log(this.state.newBookName)
      this.setState({
        books: [newBook, ...this.state.books],
        newBookName: '',
        newBookAuthor: ''
      })
    } catch {
      alert('Book creation failed')
    }
  }

  onBookDelete = async (bookId: string) => {
    try {
      await deleteBook(this.props.auth.getIdToken(), bookId)
      this.setState({
        books: this.state.books.filter(book => book.bookId !== bookId)
      })
    } catch {
      alert('Book deletion failed')
    }
  }

  onBookCheck = async (pos: number) => {
    try {
      const book = this.state.books[pos]
      await patchBook(this.props.auth.getIdToken(), book.bookId, {
        bookName: book.bookName,
        author: book.author,
        currentpageNumber: book.currentPageNumber,
        done: !book.done
      })
      this.setState({
        books: update(this.state.books, {
          [pos]: { done: { $set: !book.done } }
        })
      })
    } catch {
      alert('Book update failed')
    }
  }

  handlePageNumberChange(event: React.ChangeEvent<HTMLInputElement>, pos: number){
    const book = this.state.books[pos]
    var val = event.target.value
    console.log(val)
      this.setState({
        books: update(this.state.books, {
          [pos]: { currentPageNumber: { $set:  parseInt(val)} }
        })
      })
    
  }

  onSavePageNumber = async (pos: number) => {
     try {
      const book = this.state.books[pos]
      await patchBook(this.props.auth.getIdToken(), book.bookId, {
        bookName: book.bookName,
        author: book.author,
        currentpageNumber: book.currentPageNumber,
        done: book.done
      })
    } catch {
      alert('Book update failed')
    }
  }

  async componentDidMount() {
    try {
      const books = await getBooks(this.props.auth.getIdToken())
      this.setState({
        books,
        loadingBooks: false
      })
    } catch (e) {
      let errorMessage = "Failed to fetch books";
      if(e instanceof Error) {
      	errorMessage = e.message;
      }
      alert(errorMessage);
      
    }
  }

  render() {
    return (
      <div>
        <Header as="h1">READING LIST</Header>

        {this.renderCreateBookInput()}

        {this.renderReadingList()}
      </div>
    )
  }

  renderCreateBookInput() {
    return (
      <Grid.Row>
        <Grid.Column width={2}>
         <Button onClick={this.onBookCreate}>Add To List</Button>
        </Grid.Column>
        <Grid.Column width={7}>
          <Input
            
            fluid
            placeholder="Book Name"
            value={this.state.newBookName}
            onChange={this.handleNameChange}
          />
        </Grid.Column>
        <Grid.Column width={7}>
          <Input
            fluid
            value={this.state.newBookAuthor}
            placeholder="Book Author"
            onChange={this.handleAuthorChange}
          />
        </Grid.Column>
        <Grid.Column width={16}>
          <Divider />
        </Grid.Column>
      </Grid.Row>
    )
  }

  renderReadingList() {
    if (this.state.loadingBooks) {
      return this.renderLoading()
    }

    return this.renderBooksList()
  }

  renderLoading() {
    return (
      <Grid.Row>
        <Loader indeterminate active inline="centered">
          Loading Books
        </Loader>
      </Grid.Row>
    )
  }

  renderBooksList() {
    return (
      <Grid>
        {this.state.books.map((book, pos) => {
          return (
            <Grid.Row key={book.bookId}>
              <Grid.Column width={1} >
                <Checkbox
                  onChange={() => this.onBookCheck(pos)}
                  checked={book.done}
                />
              </Grid.Column>
              <Grid.Column width={12}>
                  <Grid.Row>
                    <h4>Book: {book.bookName}</h4>
                  </Grid.Row>
                  <Grid.Row>
                    <h4>Author: {book.author}</h4>
                  </Grid.Row>
                  <Grid.Row >
                    <Grid.Column >
                      <Input
                        action={{
                          color: 'teal',
                          labelPosition: 'right',
                          icon: 'save',
                          content: 'save',
                          onClick: () => this.onSavePageNumber(pos)
                        }}
                        fluid
                        label='Page Number'
                        onChange={(event) => {this.handlePageNumberChange(event, pos)}}
                        defaultValue={book.currentPageNumber.toString()}
                      />
                    </Grid.Column>
                  </Grid.Row>
                  <Grid.Row>
                    {book.bookCoverUrl && (
                      <Image src={book.bookCoverUrl} size="small" wrapped />
                    )}
                  </Grid.Row>
              </Grid.Column>
              <Grid.Column width={1} floated="right">
                <Button
                  icon
                  color="blue"
                  onClick={() => this.onEditButtonClick(book.bookId)}
                >
                  <Icon name="pencil" />
                </Button>
              </Grid.Column>
              <Grid.Column width={1} floated="right">
                <Button
                  icon
                  color="red"
                  onClick={() => this.onBookDelete(book.bookId)}
                >
                  <Icon name="delete" />
                </Button>
              </Grid.Column>
              <Grid.Column width={16}>
                <Divider />
              </Grid.Column>
            </Grid.Row>
          )
        })}
      </Grid>
    )
  }

}
