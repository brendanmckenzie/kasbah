import React from 'react'
import PropTypes from 'prop-types'
import { Link } from 'react-router'
import Loading from 'components/Loading'
import Error from 'components/Error'
import ContentEditor from './ContentEditor'
import SideBar from './SideBar'
import ContentTree from 'routes/Content/components/ContentTree'
import NodeForm from 'forms/NodeForm'

class View extends React.Component {
  static propTypes = {
    id: PropTypes.string.isRequired,
    getDetailRequest: PropTypes.func.isRequired,
    getDetail: PropTypes.object.isRequired,
    putDetailRequest: PropTypes.func.isRequired,
    putDetail: PropTypes.object.isRequired,
    deleteNodeRequest: PropTypes.func.isRequired,
    deleteNode: PropTypes.object.isRequired,
    putNodeRequest: PropTypes.func.isRequired,
    putNode: PropTypes.object.isRequired,
    moveNodeRequest: PropTypes.func.isRequired,
    moveNode: PropTypes.object.isRequired
  }

  static contextTypes = {
    router: PropTypes.object.isRequired,
    listTypes: PropTypes.object.isRequired
  }

  constructor() {
    super()

    this.state = {
      payload: null,
      initialLoad: true,
      showputNodeModal: false,
      showMoveNodeModal: false,
      moveNodeSelection: null
    }
  }

  componentWillMount() {
    this.handleLoad(this.props.id)
  }

  componentWillReceiveProps(nextProps) {
    if (nextProps.id !== this.props.id) {
      this.handleLoad(nextProps.id)
    }

    if (nextProps.putNode.success && nextProps.putNode.success !== this.props.putNode.success) {
      this.handleLoad(this.props.id)
      this.handleHideEditModal()
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

    if (nextProps.moveNode.success && nextProps.moveNode.success !== this.props.moveNode.success) {
      this.setState({
        showMoveNodeModal: false,
        moveNodeSelection: null
      })
    }
  }

  handleLoad = (id) => {
    this.props.getDetailRequest({ id })
  }

  handleSaveAndPublish = (data) => {
    this.props.putDetailRequest({ id: this.props.id, data, publish: true })
  }

  handleSave = (data) => {
    this.props.putDetailRequest({ id: this.props.id, data, publish: false })
  }

  handleDelete = () => {
    if (confirm('Are you sure?')) {
      if (confirm('Are you really sure?  This is a pretty destructive operation, all child nodes will be deleted.')) {
        this.props.deleteNodeRequest({ id: this.props.id })
      }
    }
  }

  handleShowEditModal = () => {
    this.setState({ showEditModal: true })
  }

  handleHideEditModal = () => {
    this.setState({ showEditModal: false })
  }

  handleEdit = (values) => {
    this.props.putNodeRequest(values)
  }

  get editModal() {
    if (!this.state.showEditModal) {
      return null
    }

    return (<NodeForm
      initialValues={this.state.payload.node}
      onClose={this.handleHideEditModal}
      onSubmit={this.handleEdit}
      types={this.context.listTypes.payload}
      loading={this.props.putNode.loading} />)
  }

  handleMoveNodeSelection = (item) => {
    this.setState({
      moveNodeSelection: item
    })
  }

  handleShowMoveNodeModal = () => {
    this.setState({
      showMoveNodeModal: true,
      moveNodeSelection: null
    })
  }

  handleHideMoveNodeModal = () => {
    this.setState({
      showMoveNodeModal: false
    })
  }

  handleMoveNode = () => {
    const { node } = this.state.payload

    if (this.state.moveNodeSelection && (this.state.moveNodeSelection.id === node.parent)) {
      // no change
      return
    }

    // update.
    this.props.moveNodeRequest({
      id: this.props.id,
      parent: this.state.moveNodeSelection ? this.state.moveNodeSelection.id : null
    })
  }

  get moveToPath() {
    if (!this.state.moveNodeSelection) {
      return (
        <ul className='breadcrumb'>
          <li>root</li>
        </ul>
      )
    }

    const { taxonomy } = this.state.moveNodeSelection

    return (
      <ul className='breadcrumb'>
        {taxonomy.aliases.map((ent, index) => (
          <li key={index}>
            <span>{ent}</span>
          </li>
        ))}
      </ul>
    )
  }

  get moveNodeModal() {
    if (!this.state.showMoveNodeModal) {
      return null
    }

    return (
      <div className='modal is-active'>
        <div className='modal-background' onClick={this.handleHideMoveNodeModal} />
        <div className='modal-card'>
          <header className='modal-card-head'>
            <span className='modal-card-title'>Move node</span>
            <button type='button' className='delete' onClick={this.handleHideMoveNodeModal} />
          </header>
          <section className='modal-card-body'>
            <ContentTree {...this.props} {...this.context} readOnly onSelect={this.handleMoveNodeSelection} />
            <hr />
            <div className='level'>
              <div className='level-left'>
                <strong className='level-item'>Move to:</strong>
                <div className='level-item'>{this.moveToPath}</div>
              </div>
            </div>
          </section>
          <footer className='modal-card-foot'>
            <button type='button' className='button' onClick={this.handleHideMoveNodeModal}>Cancel</button>
            <button type='button' className={'button is-primary ' + (this.props.moveNode.loading ? 'is-loading' : '')} onClick={this.handleMoveNode}>Save</button>
          </footer>
        </div>
      </div>
    )
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
            <SideBar {...this.state.payload}
              onDelete={this.handleDelete}
              onEdit={this.handleShowEditModal}
              onMove={this.handleShowMoveNodeModal} />
          </div>
        </div>
        {this.moveNodeModal}
        {this.editModal}
      </div>
    )
  }
}

export default View
