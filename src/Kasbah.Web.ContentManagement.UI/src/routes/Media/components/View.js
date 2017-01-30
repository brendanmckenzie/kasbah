import React from 'react'
import { Link } from 'react-router'
import moment from 'moment'
import Dropzone from 'react-dropzone'
import Loading from 'components/Loading'
import Error from 'components/Error'
import { API_BASE } from 'store/util'

class View extends React.Component {
  static propTypes = {
    listMedia: React.PropTypes.object.isRequired,
    listMediaRequest: React.PropTypes.func.isRequired,
    uploadMedia: React.PropTypes.object.isRequired,
    uploadMediaRequest: React.PropTypes.func.isRequired
  }

  componentWillMount() {
    this.handleRefresh()
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.uploadMedia.success && nextProps.uploadMedia.success !== this.props.uploadMedia.success) {
      this.handleRefresh()
    }
  }

  handleRefresh() {
    this.props.listMediaRequest()
  }

  render() {
    if (this.props.listMedia.loading) {
      return <Loading />
    }

    if (!this.props.listMedia.success) {
      return <Error />
    }

    return (
      <div className='section'>
        <div className='container'>
          <h1 className='title'>Media</h1>
          <div className='columns'>
            <div className='column is-3'>
              <ul>
                <li className='control'>
                  <Dropzone
                    onDrop={this.props.uploadMediaRequest}
                    accept='image/*'
                    style={{
                      width: 'auto',
                      height: 100,
                      border: '2px dashed #ccc',
                      backgroundColor: '#fafafa',
                      display: 'flex',
                      justifyContent: 'center',
                      alignItems: 'center',
                      cursor: 'pointer',
                      textAlign: 'center'
                    }}>
                    <div>Drag and drop files here, or click to select files to upload.</div>
                  </Dropzone>
                </li>
                {this.props.listMedia.payload.map((ent, index) => (
                  <li key={index}>
                    <Link to={`/media/${ent.id}`}>
                      <div className='media'>
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
                    </Link>
                  </li>
                ))}
              </ul>
            </div>
            <div className='column'>
              detail goes here.
            </div>
          </div>
        </div>
      </div>
    )
  }
}

export default View
