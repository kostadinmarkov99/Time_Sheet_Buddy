let draggables = document.querySelectorAll('.issue')
let containers = document.querySelectorAll('.issues-list')
let deletionTray = document.getElementById('deletion-tray')
let issuesContainer = document.getElementById('issues')
let createButton = document.getElementById('add-issue')

deletionTray.style.visibility = 'hidden'
deletionTray.style.height = '0'

createButton.addEventListener('click', e => {
    //let newIssue = createIssue("Issue Title", "Issue Description")
    //createButton.insertAdjacentElement("afterend", newIssue);
    window.location = "@(Url.Action("CreateNewTask", "Issues"))";
})

draggables.forEach(draggable => {
    //console.log("draggables.forEach - line 206");

    draggable.addEventListener('dragstart', () => {
        draggable.classList.add('dragging')
        deletionTray.style.visibility = 'visible'
        deletionTray.style.height = '30%'
        issuesContainer.style.height = '70%'
    })

    draggable.addEventListener('dragend', () => {
        draggable.classList.remove('dragging')
        deletionTray.style.visibility = 'hidden'
        deletionTray.style.height = '0'
        issuesContainer.style.height = '100%'
    })
})

containers.forEach(container => {
    container.addEventListener('dragover', e => {
        e.preventDefault()
        const afterElement = getDragAfterElement(container, e.clientY)

        const draggable = document.querySelector('.dragging')
        if (afterElement == null) {
            container.appendChild(draggable)
        } else {
            container.insertBefore(draggable, afterElement)
        }
    })
})

deletionTray.addEventListener('dragend', e => {

    //console.log("deletionTray.addEventListener line 240");
    e.preventDefault();
    deletionTray.lastChild.remove()
})

function getDragAfterElement(container, y) {
    const draggableElements = [...container.querySelectorAll('.issue:not(.dragging)')]

    return draggableElements.reduce((closest, child) => {
        const box = child.getBoundingClientRect()
        const offset = y - box.top - box.height / 2
        if (offset < 0 && offset > closest.offset) {
            return { offset: offset, element: child }
        } else {
            return closest
        }
    }, { offset: Number.NEGATIVE_INFINITY }).element
}

function issueDoubleClick(el) {
    //console.log("Hello from the issue clicked!!! " + el);

    var elInt = parseInt(el);

    window.location = "@(Url.Action("Details", "Issues"))" + "?Id=" + elInt;

    @* console.log(elInt);
    var xhr = new XMLHttpRequest();

    xhr.open('GET', '/IssueTrackers/Edit', true);
    xhr.send(elInt);*@
    }

function assignedToClick(assignedToValue) {
    // Get the modal
    var modal = document.getElementById("myModal");

    // Get Save Button
    var button = document.getElementById("saveAssignedTo");

    // Get the button that opens the modal
    var btn = document.getElementById(assignedToValue.toString().trim());
    console.log(assignedToValue.trim());
    console.log(assignedToValue);
    console.log(btn);
    // Get the <span> element that closes the modal
    var span = document.getElementsByClassName("close")[0];

    // When the user clicks the button, open the modal
    btn.onclick = function () {
        modal.style.display = "block";
    }

    // When the user clicks on <span> (x), close the modal
    span.onclick = function () {
        modal.style.display = "none";
    }

    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function (event) {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    }

    button.addEventListener('click', () => {
        var select = document.getElementById('selectAssignedTo');
        var value = select.options[select.selectedIndex].value;
        console.log(value); // en
        modal.style.display = "none";
        btn.innerText = "Assigned To: " + value;

        assignedToChange(assignedToValue, value, "assignedTo");
    })
}

function assignedToChange(assignedToValue, value, valueType) {
    console.log(assignedToValue);

    var issueId = "";
    if (valueType == "assignedTo")
        issueId = assignedToValue.substr(11, assignedToValue.length);
    else if (valueType == "project")
        issueId = assignedToValue.substr(10, assignedToValue.length);
    console.log(issueId)
    console.log(value);

    var request = new XMLHttpRequest();
    //debugger;
    var o = new Object();
    o.propVal_id = issueId;
    o.propVal_new_value = value;
    if (valueType == "assignedTo")
        o.propVal_Type = "AssigendTo";
    else if (valueType == "project")
        o.propVal_Type = "Project";

    //console.log(o.issue_id);
    //console.log(o.newState);
    //console.log(JSON.stringify(o));

    request.open('POST', '/Issues/IndexChange', true);
    request.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
    request.setRequestHeader('Content-Length', JSON.stringify(o).length);
    request.onreadystatechange = function () {
        if (request.readyState == 4 && request.status == 200) {
            //console.log(request.responseText)
        }
    }
    request.send(JSON.stringify(o));

}

function projClick(projId) {
    // Get the modal
    var modalProj = document.getElementById("myModalProj");

    // Get Save Button
    var buttonProj = document.getElementById("saveProj");

    // Get the button that opens the modal
    var btnProj = document.getElementById(projId.toString().trim());
    console.log(projId.trim());
    console.log(projId);
    console.log(btnProj);
    // Get the <span> element that closes the modal
    var spanProj = document.getElementsByClassName("closeProj")[0];

    // When the user clicks the button, open the modal
    btnProj.onclick = function () {
        modalProj.style.display = "block";
    }

    // When the user clicks on <span> (x), close the modal
    spanProj.onclick = function () {
        modalProj.style.display = "none";
    }

    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function (event) {
        if (event.target == modalProj) {
            modalProj.style.display = "none";
        }
    }

    buttonProj.addEventListener('click', () => {
        var selectProj = document.getElementById('selectProj');
        var valueProj = selectProj.options[selectProj.selectedIndex].value;
        console.log(valueProj); // en
        modalProj.style.display = "none";
        btnProj.innerText = "Project: " + valueProj;

        assignedToChange(projId, valueProj, "project");
    })
}

function createIssue(id, titleMessage, asigneeMessage, durationMessage, assignieMessage, projectMessage) {
    let newIssue = document.createElement("li");
    newIssue.classList.add('issue');
    newIssue.setAttribute("id", id);
    newIssue.setAttribute('draggable', 'true');

    newIssue.addEventListener('dragstart', () => {
        //console.log("newIssue.addEventListener dragstart - line 306")
        newIssue.classList.add('dragging')
        deletionTray.style.visibility = 'visible'
        deletionTray.style.height = '30%'
    })

    newIssue.addEventListener('dragend', () => {
        //console.log("newIssue.addEventListener draged - line 306")
        newIssue.classList.remove('dragging')
        //console.log("id of the issue is " + newIssue.id);
        deletionTray.style.visibility = 'hidden'
        deletionTray.style.height = '0'
        //console.log(document.getElementById(newIssue.id).parentNode.id);

        var innerText = document.getElementById(newIssue.id).children[1];
        //debugger;
        var state = document.getElementById(newIssue.id).parentNode.id;
        console.log(state);
        console.log(state == "closed");
        if (state == "closed") {
            innerText.innerText = "0";
        }

        var request = new XMLHttpRequest();
        //debugger;
        var o = new Object();
        o.issue_id = newIssue.id;
        o.issue_new_state = document.getElementById(newIssue.id).parentNode.id;

        //console.log(o.issue_id);
        //console.log(o.newState);
        //console.log(JSON.stringify(o));

        request.open('POST', '/Issues/IndexPost', true);
        request.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
        request.setRequestHeader('Content-Length', JSON.stringify(o).length);
        request.onreadystatechange = function () {
            if (request.readyState == 4 && request.status == 200) {
                //console.log(request.responseText)
            }
        }
        request.send(JSON.stringify(o));
    })

    let title = document.createElement('p')

    if (titleMessage == 'undefined' || titleMessage == "" || titleMessage == null) {
        titleMessage = "New Task";
    }

    title.classList.add('issue-title')
    title.setAttribute('contenteditable', 'true');
    title.innerHTML = titleMessage;

    let duration = document.createElement('p')
    duration.classList.add('issue-duration')
    duration.setAttribute('contenteditable', 'true');
    duration.innerHTML = "Duration: " + durationMessage;

    console.log(assignieMessage);

    if (assignieMessage == 'undefined' || assignieMessage == "" || assignieMessage == null) {
        assignieMessage = "UnAsigned";
    }

    let assignedToId = "assignedTo" + id;

    let assignedTo = document.createElement('p');
    assignedTo.classList.add('issue-duration');
    assignedTo.setAttribute("id", assignedToId);
    assignedTo.setAttribute('contenteditable', 'true');
    assignedTo.setAttribute('onclick', 'assignedToClick( " ' + assignedToId + ' " )');
    assignedTo.innerHTML = "Assigned To: " + assignieMessage;

    if (projectMessage == 'undefined' || projectMessage == "" || projectMessage == null) {
        projectMessage = "Choose Project";
    }

    let projId = "projectId" + id;

    let project = document.createElement('p')
    project.classList.add('issue-duration')
    project.setAttribute("id", projId);
    project.setAttribute('contenteditable', 'true');
    project.setAttribute('onclick', 'projClick( " ' + projId + ' " )');
    project.innerHTML = "Project: " + projectMessage;

    if (asigneeMessage == 'undefined' || asigneeMessage == "" || asigneeMessage == null) {
        asigneeMessage = "UnAsigned";
    }

    let assignee = document.createElement('p')
    assignee.classList.add('issue-description')
    assignee.setAttribute('contenteditable', 'true');
    assignee.innerHTML = "Assignee: " + asigneeMessage;

    newIssue.appendChild(title);
    newIssue.appendChild(duration);
    newIssue.appendChild(assignedTo);
    newIssue.appendChild(project);
    newIssue.appendChild(assignee);

    newIssue.addEventListener("dblclick", function () {
        issueDoubleClick(newIssue.id);

        var request = new XMLHttpRequest();

        request.open('POST', '/Issues/CreateNewTask', true);
        request.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
        request.setRequestHeader('Content-Length', JSON.stringify(o).length);
        request.onreadystatechange = function () {
            if (request.readyState == 4 && request.status == 200) {
                //console.log(request.responseText)
            }
        }
        request.send();
    });

    return newIssue;
}

var model = JSON.parse('@Html.Raw(Json.Serialize(Model))');
for (var i = 0; i < model.length; i++) {
    console.log(model[i]);
    let open = document.getElementById("open");
    let inProgress = document.getElementById("in-progress");
    let resolved = document.getElementById("resolved");
    let closed = document.getElementById("closed");
    if (model[i].state == "New") {

        let issue = createIssue(model[i].id, model[i].title, model[i].assignee, model[i].duration, model[i].assignedTo, model[i].project);
        open.appendChild(issue);
    }
    else if (model[i].state == "In Progress") {
        let issue = createIssue(model[i].id, model[i].title, model[i].assignee, model[i].duration, model[i].assignedTo, model[i].project);
        inProgress.appendChild(issue);
    }
    else if (model[i].state == "Active") {
        let issue = createIssue(model[i].id, model[i].title, model[i].assignee, model[i].duration, model[i].assignedTo, model[i].project);
        resolved.appendChild(issue);
    }
    else if (model[i].state == "Closed") {
        let issue = createIssue(model[i].id, model[i].title, model[i].assignee, "0", model[i].assignedTo, model[i].project);
        closed.appendChild(issue);
    }
}