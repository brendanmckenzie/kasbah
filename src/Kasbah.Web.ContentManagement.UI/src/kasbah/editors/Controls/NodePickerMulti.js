import React from 'react'
import PropTypes from 'prop-types'
import moment from 'moment'
import _ from 'lodash'
import { NavLink } from 'react-router-dom'
import Loading from 'components/Loading'
import { makeApiRequest } from 'store/util'

class NodePickerMulti extends React.Component {
  static propTypes = {
    input: PropTypes.object.isRequired,
    type: PropTypes.string
  }

  static alias = 'nodePickerMulti'

  constructor() {
    super()

    this.state = {
      showModal: false,
      loading: true,
      nodes: [],
      selection: [],
      nodeDetail: {}
    }
  }

  componentWillMount() {
    this.handleReloadNodeDetail(this.props.input.value)
  }

  handleRefresh = () => {
    makeApiRequest({
      url: '/content/nodes/by-type',
      method: 'POST',
      body: {
        type: this.props.type,
        inherit: true
      }
    })
      .then(nodes => {
        this.setState({
          loading: false,
          nodes
        })
      })
  }

  handleShowModal = () => {
    this.handleRefresh()
    this.setState({
      showModal: true
    })
  }

  handleHideModal = () => {
    this.setState({
      showModal: false
    })
  }

  handleSelect = (item) => {
    this.setState({
      selection: this.state.selection.indexOf(item) === -1
        ? [...this.state.selection, item]
        : this.state.selection.filter(ent => ent !== item)
    })
  }

  handleCommit = () => {
    const { input: { onChange } } = this.props

    onChange(this.state.selection)

    this.handleReloadNodeDetail(this.state.selection)

    this.handleHideModal()
  }

  handleReloadNodeDetail = (value) => {
    if (!value) { return }

    Promise.all(value.map(ent => makeApiRequest({ url: `/content/node/${ent}`, method: 'GET' })))
      .then(res => {
        let detail = {}
        res.forEach(ent => {
          detail[ent.id] = ent
        })
        this.setState({
          nodeDetail: detail
        })
      })
  }

  handleClear = () => {
    const { input: { onChange } } = this.props

    onChange([])
    this.setState({
      selection: []
    })
  }

  renderNode(id) {
    if (this.state.nodeDetail[id]) {
      const item = this.state.nodeDetail[id]
      return (
        <div className='level'>
          <div className='level-left'>
            <div className='level-item'>
              <p><NavLink to={`/content/${id}`}>{item.displayName}</NavLink></p>
              <p><small>{item.alias}</small></p>
            </div>
          </div>
          <div className='level-right'>
            <span className={`tag ` + (item.publishedVersion ? ' is-primary' : ' is-warning')}>
              {item.publishedVersion ? `v${item.publishedVersion}` : 'not published'}
            </span>
          </div>
        </div>
      )
    } else {
      return (
        <NavLink to={`/content/${id}`}>{id}</NavLink>
      )
    }
  }

  get display() {
    const { input: { value } } = this.props

    if (!value) { return null }

    return (<ul>
      {value.map(ent => (
        <li key={ent}>
          {this.renderNode(ent)}
        </li>
      ))}
    </ul>)
  }

  get modal() {
    if (!this.state.showModal) { return null }

    return (<div className='modal is-active'>
      <div className='modal-background' />
      <div className='modal-card'>
        <header className='modal-card-head'>
          <span className='modal-card-title'>
            Node selector
          </span>
        </header>
        <section className='modal-card-body'>
          {this.state.loading ? <Loading /> : (
            _(this.state.nodes).sortBy('modified').reverse().value().map(ent => (
              <div key={ent.id} className='field'>
                <div
                  className={'card' + (this.state.selection.indexOf(ent.id) === -1 ? '' : ' is-selected')}
                  onClick={() => this.handleSelect(ent.id)}>
                  <div className='card-content'>
                    <p><strong>{ent.displayName}</strong> <small>{ent.alias}</small></p>
                    <p><small>{moment(ent.modified).fromNow()}</small></p>
                  </div>
                </div>
              </div>
            ))
          )}
        </section>
        <footer className='modal-card-foot'>
          <button type='button' className='button is-primary' onClick={this.handleCommit}>Select</button>
          <button type='button' className='button' onClick={this.handleHideModal}>Cancel</button>
        </footer>
      </div>
    </div>)
  }

  render() {
    return (<div className='node-picker'>
      {this.display}
      <div className='level'>
        <div className='level-left' />
        <div className='level-right'>
          <button type='button' className='level-item button is-small' onClick={this.handleClear}>Clear</button>
          <button
            type='button'
            className='level-item button is-small'
            onClick={this.handleShowModal}>Select nodes</button>
        </div>
      </div>
      {this.modal}
    </div>)
  }
}

export default NodePickerMulti
