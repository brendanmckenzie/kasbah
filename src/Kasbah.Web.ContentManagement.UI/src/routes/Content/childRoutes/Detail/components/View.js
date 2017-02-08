import React from 'react'
import { Link } from 'react-router'
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
    deleteNodeRequest: React.PropTypes.func.isRequired,
    deleteNode: React.PropTypes.object.isRequired
  }

  static contextTypes = {
    router: React.PropTypes.object.isRequired
  }

  constructor() {
    super()

    this.state = {
      payload: null,
      initialLoad: true
    }

    this.handleSaveAndPublish = this.handleSaveAndPublish.bind(this)
    this.handleSave = this.handleSave.bind(this)
    this.handleDelete = this.handleDelete.bind(this)
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
        initialLoad: false,
        payload: nextProps.getDetail.payload
      })
    }

    if (nextProps.putDetail.success && nextProps.putDetail.success !== this.props.putDetail.success) {
      this.setState({
        payload: nextProps.putDetail.payload
      })
    }

    if (nextProps.deleteNode.success && nextProps.deleteNode.success !== this.props.deleteNode.success) {
      if (this.state.payload.node.parent) {
        this.context.router.push(`/content/${this.state.payload.node.parent}`)
      } else {
        this.context.router.push('/content')
      }
    }
  }

  handleLoad(id) {
    this.props.getDetailRequest({ id })
  }

  handleSaveAndPublish(data) {
    this.props.putDetailRequest({ id: this.props.id, data, publish: true })
  }

  handleSave(data) {
    this.props.putDetailRequest({ id: this.props.id, data, publish: false })
  }

  handleDelete() {
    if (confirm('Are you sure?')) {
      if (confirm('Are you really sure?  This is a pretty destructive operation, all child nodes will be deleted.')) {
        this.props.deleteNodeRequest({ id: this.props.id })
      }
    }
  }

  get breadcrumb() {
    const { taxonomy, id } = this.state.payload.node

    return (<ul className='breadcrumb'>
      {taxonomy.aliases.map((ent, index) => (
        <li key={index}>
          {taxonomy.ids[index] === id
            ? (<span>{ent}</span>)
            : (<Link to={`/content/${taxonomy.ids[index]}`}>{ent}</Link>)}
        </li>
      ))}
    </ul>)
  }

  render() {
    if (this.props.getDetail.loading || this.state.initialLoad) {
      return <Loading />
    }

    if (!this.props.getDetail.success) {
      return <Error />
    }

    return (
      <div>
        <h2 className='subtitle'>{this.state.payload.node.displayName}</h2>
        {this.breadcrumb}
        <div className='columns'>
          <div className='column'>
            {this.state.payload.type.fields.length === 0
              ? <p>This content type does not define any editable fields.</p>
              : <ContentEditor
                payload={this.state.payload}
                loading={this.props.putDetail.loading}
                onSubmit={this.handleSaveAndPublish}
                onSave={this.handleSave} />}
          </div>
          <div className='column is-3'>
            <SideBar {...this.state.payload} onDelete={this.handleDelete} />
          </div>
        </div>
      </div>
    )
  }
}

export default View
