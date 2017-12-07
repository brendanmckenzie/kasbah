import React from 'react'
import PropTypes from 'prop-types'
import Dropzone from 'react-dropzone'
import Loading from 'components/Loading'
import MediaList from './MediaList'
import { Section, Container } from 'components/Layout'

class View extends React.Component {
  static propTypes = {
    media: PropTypes.object.isRequired,
    listMedia: PropTypes.func.isRequired,
    uploadMedia: PropTypes.func.isRequired
  }

  componentDidMount() {
    if (!this.props.media.loaded) {
      this.handleRefresh()
    }
  }

  handleRefresh() {
    this.props.listMedia()
  }

  get upload() {
    if (this.props.media.uploading) {
      return <Loading message='An upload is in progress. Please wait...' />
    }

    return (
      <div className='control'>
        <div className='message'>
          <Dropzone
            onDrop={this.props.uploadMedia}
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
      <Section>
        <Container>
          {this.upload}
          <hr />
          <MediaList media={this.props.media} />
        </Container>
      </Section>
    )
  }
}

export default View
