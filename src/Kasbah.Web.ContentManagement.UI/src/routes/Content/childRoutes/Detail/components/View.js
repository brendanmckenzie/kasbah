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
    putDetail: React.PropTypes.object.isRequired
  }

  constructor() {
    super()

    this.state = { payload: null }

    this.handleSaveAndPublish = this.handleSaveAndPublish.bind(this)
    this.handleSave = this.handleSave.bind(this)
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
  }

  handleLoad(id) {
    this.props.getDetailRequest({ id })
  }

  handleSaveAndPublish(data) {
    this.props.putDetailRequest({ id: this.props.id, data, publish: true })
  }

  handleSave() {
    this.props.putDetailRequest({ id: this.props.id, publish: false })
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
    if (this.props.getDetail.loading) {
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
            <SideBar {...this.state.payload} />
          </div>
        </div>
      </div>
    )
  }
}

export default View
