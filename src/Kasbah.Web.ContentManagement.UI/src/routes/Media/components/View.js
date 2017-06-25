import React from 'react'
import PropTypes from 'prop-types'
import Dropzone from 'react-dropzone'
import Loading from 'components/Loading'
import Error from 'components/Error'
import MediaItem from './MediaItem'

class View extends React.Component {
  static propTypes = {
    listMedia: PropTypes.object.isRequired,
    listMediaRequest: PropTypes.func.isRequired,
    uploadMedia: PropTypes.object.isRequired,
    uploadMediaRequest: PropTypes.func.isRequired,
    deleteMedia: PropTypes.object.isRequired,
    deleteMediaRequest: PropTypes.func.isRequired,
    putMedia: PropTypes.object.isRequired,
    putMediaRequest: PropTypes.func.isRequired,
    media: PropTypes.object.isRequired,
    setLoaded: PropTypes.func.isRequired
  }

  componentWillMount() {
    if (!this.props.media.loaded) {
      this.handleRefresh()
    }
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.uploadMedia.success && nextProps.uploadMedia.success !== this.props.uploadMedia.success) {
      this.handleRefresh()
    }

    if (nextProps.deleteMedia.success && nextProps.deleteMedia.success !== this.props.deleteMedia.success) {
      this.handleRefresh()
    }

    if (nextProps.putMedia.success && nextProps.putMedia.loading !== this.props.putMedia.loading) {
      this.handleRefresh()
    }

    if (nextProps.listMedia.success && nextProps.listMedia.loading !== this.props.listMedia.loading) {
      this.props.setLoaded(true)
    }
  }

  handleRefresh() {
    this.props.listMediaRequest()
  }

  handleDelete = (ent) => {
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
          <MediaItem
            key={ent.id}
            item={ent}
            onDelete={this.handleDelete}
            onEdit={this.props.putMediaRequest}
            putMedia={this.props.putMedia} />
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
          {this.upload}
          <hr />
          {this.list}
        </div>
      </div>
    )
  }
}

export default View
