A Reception Desk Without the Paperwork

Anyone who has ever queued at a clinic knows that the scarcest resource in medicine is not equipment but time: a doctor hunting for a patient file, a patient waiting at the window, an administrator reconciling the schedule with a paper notebook.

Clinic Service is a clinic registry system that brings all of this into one place. Administrators register patients, insurers and policies, maintain medical cards and book appointments in a clear weekly calendar. A doctor signs in and sees only what matters: their own upcoming appointments and a "Current appointment" tab showing the visit that is under way or about to begin. With one click they start the appointment, open the patient's card and add a note: the date, the complaints, the diagnosis. A new diagnosis is added to the card automatically and shown the way doctors expect it: "ICD code - name".

Security is not an option. Medical data does not forgive carelessness. Sign-in relies on short-lived, rotating tokens, lockout after failed attempts and TOTP two-factor authentication. An administrator creates a user with an initial password, and the user must choose their own at first login. Administrators can switch two-factor protection on or off for any doctor.

A history that cannot be erased. Data in the system is a chain of versions: an edit never overwrites what was there, it adds a new state. Deleted records can be restored, and every change leaves a trace. For clinical documentation this is not a luxury but a condition of trust.

Under the hood are ASP.NET Core 10 and Angular, a documented OpenAPI contract, and a database of your choice: SQLite to try it out, SQL Server or PostgreSQL for real work. The project is at version 0.1.0: the beginning of the road, but the foundation is already solid.

Регистратура без бумажной волокиты

Каждый, кто хоть раз стоял в очереди в поликлинику, знает: самое дорогое в медицине — не оборудование, а время. Время врача, который ищет карту. Время пациента, который ждёт у окошка. Время администратора, который сверяет расписание с тетрадью.

Clinic Service — это система регистратуры, которая собирает всё это в одном месте. Администратор заводит пациентов, страховые компании и полисы, ведёт медицинские карты и записывает людей на приём в наглядном недельном календаре. Врач входит в систему и сразу видит своё: только свои будущие приёмы и вкладку «Текущий приём» — тот, что идёт сейчас или вот-вот начнётся. Одной кнопкой он начинает приём, открывает карту пациента и вносит запись: дату, жалобы, диагноз. Новый диагноз сам попадает в карту и всегда показывается так, как привыкли врачи: «код МКБ — название».

Безопасность — не опция. Медицинские данные не терпят небрежности, поэтому вход защищён токенами с коротким сроком жизни и ротацией, блокировкой после неудачных попыток и двухфакторной аутентификацией по TOTP. Администратор создаёт пользователя с начальным паролем, а тот при первом входе обязан задать свой. Двухфакторную защиту администратор может включить и выключить для любого врача.

История, которую нельзя стереть. Данные в системе устроены как цепочка версий: правка не затирает прежнее состояние, а добавляет новое. Удалённое можно восстановить, а каждое изменение оставляет след. Для клинической документации это не роскошь, а условие доверия.

Под капотом — ASP.NET Core 10 и Angular, документированное API (OpenAPI), база на выбор: SQLite для пробы, SQL Server или PostgreSQL для настоящей работы. Версия проекта — 0.1.0: это начало пути, но основа уже прочная.
