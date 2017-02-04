import React from 'react'
import moment from 'moment'
import Loading from 'components/Loading'
import { API_BASE, makeApiRequest } from 'store/util'

class MediaPicker extends React.Component {
  static propTypes = {
    input: React.PropTypes.object.isRequired,
    options: React.PropTypes.object
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

    this.handleHideModal = this.handleHideModal.bind(this)
    this.handleShowModal = this.handleShowModal.bind(this)
    this.handleSelect = this.handleSelect.bind(this)
    this.handleCommit = this.handleCommit.bind(this)
  }

  componentWillMount() {
    makeApiRequest({ url: '/media/list', method: 'GET' })
      .then(media => {
        this.setState({
          loading: false,
          media
        })
      })
  }

  handleShowModal() {
    this.setState({
      showModal: true
    })
  }

  handleHideModal() {
    this.setState({
      showModal: false
    })
  }

  handleSelect(item) {
    this.setState({
      selection: item
    })
  }

  handleCommit() {
    const { input: { onChange } } = this.props

    onChange(this.state.selection)

    this.handleHideModal()
  }

  get display() {
    const { input: { value } } = this.props

    if (!value) { return null }

    return (<div>
      <img src={`${API_BASE}/media/${value}?width=600&height=450`} />
    </div>)
  }

  get modal() {
    return (<div className='modal is-active'>
      <div className='modal-background' />
      <div className='modal-card'>
        <header className='modal-card-head'>
          <span className='modal-card-title'>
            Media picker
          </span>
        </header>
        <section className='modal-card-body'>
          <div className='columns is-multiline'>
            {this.state.loading ? <Loading /> : (
              this.state.media.map(ent => (
                <div key={ent.id} className='column is-4'>
                  <div
                    className={'media ' + (this.state.selection === ent.id ? 'is-selected' : '')}
                    onClick={() => this.handleSelect(ent.id)}>
                    <figure className='media-left'>
                      <span className='image is-64x64'>
                        <img src={`${API_BASE}/media/${ent.id}?width=128&height=128`} />
                      </span>
                    </figure>
                    <div className='media-content'>
                      <p><strong>{ent.fileName}</strong></p>
                      <p>{ent.contentType}</p>
                      <p><small>{moment(ent.created).fromNow()}</small></p>
                    </div>
                  </div>
                </div>
              ))
            )}
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
      <div className='has-text-right'>
        <button type='button' className='button is-small' onClick={this.handleShowModal}>
          Select media
        </button>
      </div>
      {this.state.showModal && this.modal}
    </div>)
  }
}

export default MediaPicker
