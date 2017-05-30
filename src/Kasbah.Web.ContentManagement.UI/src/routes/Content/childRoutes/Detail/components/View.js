import React from 'react'
import { Link } from 'react-router'
import Loading from 'components/Loading'
import Error from 'components/Error'
import ContentEditor from './ContentEditor'
import SideBar from './SideBar'
import ContentTree from 'routes/Content/components/ContentTree'

class View extends React.Component {
  static propTypes = {
    id: React.PropTypes.string.isRequired,
    describeTreeRequest: React.PropTypes.func.isRequired,
    describeTree: React.PropTypes.object.isRequired,
    getDetailRequest: React.PropTypes.func.isRequired,
    getDetail: React.PropTypes.object.isRequired,
    putDetailRequest: React.PropTypes.func.isRequired,
    putDetail: React.PropTypes.object.isRequired,
    deleteNodeRequest: React.PropTypes.func.isRequired,
    deleteNode: React.PropTypes.object.isRequired,
    updateNodeAliasRequest: React.PropTypes.func.isRequired,
    updateNodeAlias: React.PropTypes.object.isRequired,
    changeTypeRequest: React.PropTypes.func.isRequired,
    changeType: React.PropTypes.object.isRequired,
    moveNodeRequest: React.PropTypes.func.isRequired,
    moveNode: React.PropTypes.object.isRequired
  }

  static contextTypes = {
    router: React.PropTypes.object.isRequired
  }

  constructor() {
    super()

    this.state = {
      payload: null,
      initialLoad: true,
      showChangeTypeModal: false,
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

    if (nextProps.updateNodeAlias.success && nextProps.updateNodeAlias.success !== this.props.updateNodeAlias.success) {
      this.handleLoad(this.props.id)
    }

    if (nextProps.changeType.success && nextProps.changeType.success !== this.props.changeType.success) {
      this.handleLoad(this.props.id)
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

  handleRename = () => {
    const { node } = this.state.payload

    const alias = prompt('What should we rename this to?', node.alias)
    if (!alias || alias === node.alias) {
      // no change
      return
    }

    // update.
    this.props.updateNodeAliasRequest({
      id: this.props.id,
      alias
    })
  }

  handleChangeType = () => {
    const { node } = this.state.payload

    const type = prompt('What type should we give this node?', node.type)
    if (!type || type === node.type) {
      // no change
      return
    }

    // update.
    this.props.changeTypeRequest({
      id: this.props.id,
      type
    })
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

  get changeTypeModal() {
    if (!this.state.showChangeTypeModal) {
      return null
    }
    return null
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
            <ContentTree {...this.props} readOnly onSelect={this.handleMoveNodeSelection} />
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
              onRename={this.handleRename}
              onChangeType={this.handleChangeType}
              onMove={this.handleShowMoveNodeModal} />
          </div>
        </div>
        {this.moveNodeModal}
      </div>
    )
  }
}

export default View
