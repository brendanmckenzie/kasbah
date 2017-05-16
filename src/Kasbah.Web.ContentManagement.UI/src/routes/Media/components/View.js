import React from 'react'
import moment from 'moment'
import Dropzone from 'react-dropzone'
import Loading from 'components/Loading'
import Error from 'components/Error'
import ItemButton from 'components/ItemButton'
import { API_BASE } from 'store/util'

class View extends React.Component {
  static propTypes = {
    listMedia: React.PropTypes.object.isRequired,
    listMediaRequest: React.PropTypes.func.isRequired,
    uploadMedia: React.PropTypes.object.isRequired,
    uploadMediaRequest: React.PropTypes.func.isRequired,
    deleteMedia: React.PropTypes.object.isRequired,
    deleteMediaRequest: React.PropTypes.func.isRequired
  }

  constructor() {
    super()

    this.handleDelete = this.handleDelete.bind(this)
  }

  componentWillMount() {
    this.handleRefresh()
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.uploadMedia.success && nextProps.uploadMedia.success !== this.props.uploadMedia.success) {
      this.handleRefresh()
    }

    if (nextProps.deleteMedia.success && nextProps.deleteMedia.success !== this.props.deleteMedia.success) {
      this.handleRefresh()
    }
  }

  handleRefresh() {
    this.props.listMediaRequest()
  }

  handleDelete(ent) {
    if (confirm('Are you sure?')) {
      this.props.deleteMediaRequest(ent)
    }
  }

  get list() {
    if (this.props.listMedia.loading) {
      return <Loading />
    }

    if (!this.props.listMedia.success) {
      return <Error />
    }

    return (
      <div className='columns is-multiline'>
        {this.props.listMedia.payload.map((ent, index) => (
          <div key={ent.id} className='column is-4'>
            <div className='media'>
              <figure className='media-left'>
                <span className='image is-64x64'>
                  <img src={`${API_BASE}/media?id=${ent.id}&width=128&height=128`} />
                </span>
              </figure>
              <div className='media-content'>
                <p><strong>{ent.fileName}</strong></p>
                <p>{ent.contentType}</p>
                <p><small>{moment(ent.created).fromNow()}</small></p>
              </div>
              <div className='media-right'>
                <ItemButton className='delete' onClick={this.handleDelete} item={ent} />
              </div>
            </div>
          </div>
        ))}
      </div>
    )
  }

  get upload() {
    if (this.props.uploadMedia.loading) {
      return <Loading message='An upload is in progress. Please wait...' />
    }

    return (
      <div className='control'>
        <div className='message'>
          <Dropzone
            onDrop={this.props.uploadMediaRequest}
            accept='image/*'
            className='message-body media--upload__dropzone'
            style={{ cursor: 'pointer' }}>
            <p><i className='fa fa-upload' /> Drag and drop files here, or click to select files to upload.</p>
          </Dropzone>
        </div>
      </div>
    )
  }

  render() {
    return (
      <div className='section'>
        <div className='container'>
          <h1 className='title'>Media</h1>
          {this.upload}
          <hr />
          {this.list}
        </div>
      </div>
    )
  }
}

export default View
