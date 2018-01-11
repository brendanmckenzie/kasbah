import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import _ from 'lodash'
import moment from 'moment'
import { API_BASE, makeApiRequest } from 'store/util'
import { actions as mediaActions } from 'store/appReducers/media'
import { createFilterFunc } from './Shared/search'

class MediaPicker extends React.Component {
  static propTypes = {
    input: PropTypes.object.isRequired,
    media: PropTypes.object.isRequired,
    listMedia: PropTypes.func.isRequired
  }

  static alias = 'mediaPicker'

  constructor() {
    super()

    this.state = {
      showModal: false,
      loading: true,
      media: [],
      selection: null,
      search: ''
    }
  }

  componentWillMount() {
    this.handleReloadMediaDetail(this.props.input.value)

    if (!this.props.media.list.loaded && !this.props.media.list.loading) {
      this.props.listMedia()
    }
  }

  handleShowModal = () => {
    this.setState({
      showModal: true,
      loading: true,
      selection: this.props.input.value
    })
  }

  handleHideModal = () => {
    this.setState({
      showModal: false
    })
  }

  handleSelect = (item) => {
    this.setState({
      selection: item
    })
  }

  handleCommit = () => {
    const { input: { onChange } } = this.props

    onChange(this.state.selection)

    this.handleReloadMediaDetail(this.state.selection)

    this.handleHideModal()
  }

  handleClear = () => {
    const { input: { onChange } } = this.props

    onChange(null)
    this.setState({
      selection: null
    })
  }

  handleReloadMediaDetail = (value) => {
    if (!value) { return }

    makeApiRequest({ url: `/media/${value}/meta`, method: 'GET' })
      .then(mediaDetail => {
        this.setState({
          mediaDetail
        })
      })
  }

  handleSearchChange = (ev) => {
    this.setState({
      search: ev.target.value
    })
  }

  get display() {
    const { input: { value } } = this.props

    if (!value) { return <p>No media selected.</p> }

    if (this.state.mediaDetail) {
      return (
        <div className='media'>
          {this.state.mediaDetail.contentType.startsWith('image/') && (
            <div className='media-left'>
              <figure className='image is-128x128'>
                <img src={`${API_BASE}/media?id=${value}&width=256&height=256`} />
              </figure>
            </div>
          )}
          <div className='media-content'>
            <p><strong>{this.state.mediaDetail.fileName}</strong></p>
            <p><small>{this.state.mediaDetail.contentType}</small></p>
            <p><small>{moment.utc(this.state.mediaDetail.created).fromNow()}</small></p>
          </div>
        </div>
      )
    } else {
      return (
        <div className='media'>
          <div className='media-content'>
            <p>{value}</p>
          </div>
        </div>
      )
    }
  }

  get filteredMedia() {
    const filterFunc = createFilterFunc(['fileName', 'contentType'])
    const filter = (ent) => filterFunc(this.state.search, ent)

    return _(this.props.media.list.items).filter(filter).sortBy('created').reverse().value()
  }

  get modal() {
    return (<div className='modal is-active'>
      <div className='modal-background' />
      <div className='modal-card'>
        <header className='modal-card-head'>
          <span className='modal-card-title'>
            Media picker
          </span>
          <button type='button' className='delete' onClick={this.handleHideModal} />
        </header>
        <section className='modal-card-body'>
          <div className='field'>
            <div className='control'>
              <input type='search' className='input' autoFocus
                onChange={this.handleSearchChange} value={this.state.search} />
            </div>
          </div>
          <div className='columns is-multiline'>
            {this.filteredMedia.map(ent => (
              <div key={ent.id} className='column is-4'>
                <div
                  className={'card ' + (this.state.selection === ent.id ? 'is-selected' : '')}
                  onClick={() => this.handleSelect(ent.id)}>
                  <figure className='card-image'>
                    <span className='image is-4by3'>
                      <img src={`${API_BASE}/media?id=${ent.id}&width=256&height=192`} />
                    </span>
                  </figure>
                  <div className='card-content'>
                    <p className='filename' title={ent.fileName}><strong>{ent.fileName}</strong></p>
                    <p><small>{ent.contentType}</small></p>
                    <p><small>{moment.utc(ent.created).fromNow()}</small></p>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </section>
        <footer className='modal-card-foot'>
          <button type='button' className='button is-primary' onClick={this.handleCommit}>Select</button>
          <button type='button' className='button' onClick={this.handleHideModal}>Cancel</button>
        </footer>
      </div>
    </div>)
  }

  render() {
    return (<div className='media-picker'>
      {this.display}
      <div className='level'>
        <div className='level-left' />
        <div className='level-right'>
          <button type='button' className='level-item button is-small' onClick={this.handleClear}>Clear</button>
          <button
            type='button'
            className='level-item button is-primary is-small'
            onClick={this.handleShowModal}>Select media</button>
        </div>
      </div>
      {this.state.showModal && this.modal}
    </div>)
  }
}

const mapStateToProps = (state) => ({
  media: state.media,
})

const mapDispatchToProps = {
  ...mediaActions
}

export default connect(mapStateToProps, mapDispatchToProps)(MediaPicker)
