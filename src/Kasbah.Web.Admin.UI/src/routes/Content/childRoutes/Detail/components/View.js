import React from 'react'
import Loading from 'components/Loading'
import ContentEditor from './ContentEditor'
import SideBar from './SideBar'

class View extends React.Component {
  static propTypes = {
    id: React.PropTypes.string.isRequired,
    getDetailRequest: React.PropTypes.func.isRequired,
    getDetail: React.PropTypes.object.isRequired,
    putDetailRequest: React.PropTypes.func.isRequired,
    putDetail: React.PropTypes.object.isRequired,
    publishRequest: React.PropTypes.func.isRequired,
    publish: React.PropTypes.object.isRequired
  }

  constructor() {
    super()

    this.handleSave = this.handleSave.bind(this)
    this.handlePublish = this.handlePublish.bind(this)
  }

  componentWillMount() {
    this.handleLoad(this.props.id)
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.id !== this.props.id) {
      this.handleLoad(nextProps.id)
    }
  }

  handleLoad(id) {
    this.props.getDetailRequest({ id })
  }

  handleSave(data) {
    this.props.putDetailRequest({ id: this.props.id, data })
  }

  handlePublish() {
    this.props.publishRequest({ id: this.props.id, version: this.props.getDetail.payload.data['_version'] })
  }

  render() {
    if (this.props.getDetail.loading) {
      return <Loading />
    }

    return (
      <div>
        <h2 className='subtitle'>{this.props.getDetail.payload.node.displayName}</h2>
        <div className='columns'>
          <div className='column'>
            <ContentEditor {...this.props.getDetail}
              loading={this.props.putDetail.loading}
              onSubmit={this.handleSave}
              onPublish={this.handlePublish} />
          </div>
          <div className='column is-3'>
            <SideBar {...this.props.getDetail.payload} />
          </div>
        </div>
      </div>
    )
  }
}

export default View
