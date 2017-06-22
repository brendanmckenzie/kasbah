import React from 'react'
import PropTypes from 'prop-types'
import _ from 'lodash'
import moment from 'moment'
import Loading from 'components/Loading'
import { API_BASE, makeApiRequest } from 'store/util'

class MediaPicker extends React.Component {
  static propTypes = {
    input: PropTypes.object.isRequired,
    options: PropTypes.object
  }

  static alias = 'mediaPicker'

  constructor() {
    super()

    this.state = {
      showModal: false,
      loading: true,
      media: [],
      selection: null
    }
  }

  componentWillMount() {
    this.handleReloadMediaDetail(this.props.input.value)
  }

  handleShowModal = () => {
    this.setState({
      showModal: true,
      loading: true,
      selection: this.props.input.value
    })

    makeApiRequest({ url: '/media/list', method: 'GET' })
      .then(media => {
        this.setState({
          loading: false,
          media
        })
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
            <p><small>{moment(this.state.mediaDetail.created).fromNow()}</small></p>
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
          {this.state.loading ? <Loading /> : (
            <div className='columns is-multiline'>
              {_(this.state.media).sortBy('created').reverse().map(ent => (
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
                      <p><small>{moment(ent.created).fromNow()}</small></p>
                    </div>
                  </div>
                </div>
              )).value()}
            </div>
          )}
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
      <div className='has-text-right'>
        <button type='button' className='button is-small' onClick={this.handleClear}>Clear</button>
        <button type='button' className='button is-primary is-small' onClick={this.handleShowModal}>Select media</button>
      </div>
      {this.state.showModal && this.modal}
    </div>)
  }
}

export default MediaPicker
