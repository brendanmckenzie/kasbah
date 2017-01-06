import React from 'react'
import Loading from 'components/Loading'
import Error from 'components/Error'
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

    this.state = { payload: null }

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

    if (nextProps.getDetail.success && nextProps.getDetail.success !== this.props.getDetail.success) {
      this.setState({
        payload: nextProps.getDetail.payload
      })
    }

    if (nextProps.putDetail.success && nextProps.putDetail.success !== this.props.putDetail.success) {
      this.setState({
        payload: nextProps.putDetail.payload
      })
    }

    if (nextProps.publish.success && nextProps.publish.success !== this.props.publish.success) {
      this.setState({
        payload: nextProps.publish.payload
      })
    }
  }

  handleLoad(id) {
    this.props.getDetailRequest({ id })
  }

  handleSave(data) {
    this.props.putDetailRequest({ id: this.props.id, data })
  }

  handlePublish() {
    this.props.publishRequest({ id: this.props.id, version: this.state.payload.data['_version'] })
  }

  render() {
    if (this.props.getDetail.loading) {
      return <Loading />
    }

    if (!this.props.getDetail.success) {
      return <Error />
    }

    return (
      <div>
        <h2 className='subtitle'>{this.state.payload.node.displayName}</h2>
        <ul className='breadcrumb'>
          {this.state.payload.node.taxonomy.aliases.map((ent, index) => <li key={index}>{ent}</li>)}
        </ul>
        <div className='columns'>
          <div className='column'>
            <ContentEditor
              payload={this.state.payload}
              loading={this.props.putDetail.loading}
              publishing={this.props.publish.loading}
              onSubmit={this.handleSave}
              onPublish={this.handlePublish} />
          </div>
          <div className='column is-3'>
            <SideBar {...this.state.payload} />
          </div>
        </div>
      </div>
    )
  }
}

export default View
